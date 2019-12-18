USE [data_gather]
GO
/****** Object:  StoredProcedure [dbo].[MES_ProcessDeviceData]    Script Date: 2019/12/17 14:39:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[MES_ProcessDeviceData] 
    -- 1. 如果是按键，则读取品质代码表，进行不良报工
    -- 2. 如果光感，则进行良品报工    
(
    @GID             int,
    @DID             int,
    @ReqDataType     int,
    @ReqData         varchar(20),
    @ReqTime         datetime,
    @RespData        varchar(200)   output
)as
begin
    declare @DefectCode varchar(20),@WorkstationCode varchar(20), @WorkstationName varchar(50), @WorkorderNo varchar(20),@LineNo varchar(20);
    declare @RecordType int,@SpanId int,@DefectReportMethod int,@ProductDate datetime,@PartNo varchar(50);
    declare @QtyGood int,@QtyBad int,@QtyPlan int,@lock int,@Hours int,@IsLastWorkstation bit,@WorkstationSeq int;
	declare @LineId bigint,@WorkshiftCode varchar(20),@WorkshopId bigint,@OrgType varchar(20),@Lamp int;    
    
    select @RespData ='',@DefectCode='',@WorkstationCode='',@WorkorderNo='',@RecordType=-1,@SpanId=-1,
           @QtyGood = 0,@QtyBad = 0 ,@QtyPlan = 0;
	   
	if (select count(*)  from mes_org w
    where w.org_type = 'ORG_WORK_STATION'
      and w.gid = @GID
      and w.did = @DID )>1 begin
		set @RespData = '2|1|4'; 
        set @RespData = @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
		set @RespData = @RespData + '|1|组号:' + cast(@GID as varchar(10)) + ',机号:' + cast(@DID as varchar(10)) +'|0';
		set @RespData = @RespData + '|2|工位重复注册|0';
		set @RespData = @RespData + '|3|请联系管理员|0';

		return;
	end;

    select @WorkstationCode = org_code,@LineId = parent_id, @WorkstationName = org_name, 
          @DefectReportMethod = defect_report_method,@WorkstationSeq = seq,
          @OrgType = org_type
        from mes_org where gid = @GID and did = @DID;
    if(@WorkstationCode = '')begin
		set @RespData ='2|1|4';
		set @RespData = @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
        set @RespData = @RespData + '|1|组号:' + cast(@GID as varchar(10)) + ',机号:' + cast(@DID as varchar(10)) +'|0';
		set @RespData = @RespData + '|2|工位没有注册.|0';
        set @RespData = @RespData + '|3|请联系管理员|0';

        return;
    end;

    if(@OrgType='ORG_WORK_LINE') begin
        if (not @ReqData in('1','3')) begin
            set @RespData ='2|1|3';
            set @RespData = @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
            set @RespData = @RespData + '|1|红灯按1|0';
            set @RespData = @RespData + '|2|绿灯按3.|0';                  
        end else begin
            set @Lamp = cast(@ReqData as int);
            exec MES_Light @GID,@DID,@Lamp;

            set @RespData ='2|1|3';
            set @RespData = @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
            set @RespData = @RespData + '|1|已处理，|0';
            set @RespData = @RespData + '|2|请查看灯|0';             
        end;
        return;
    end;
    
	
    select @LineNo = L.org_code,@WorkshopId = parent_id  from mes_org  L  where L.record_id = @LineId;
    select top 1 @WorkorderNo = workorder_no,@PartNo = part_no  from mes_active_workorder A   where A.line_no = @LineNo;
    if @WorkorderNo = ''  begin
        set @RespData='2|1|2';
        set @RespData= @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
        set @RespData= @RespData + '|1|还没有启用工单.|0';         

        return;
    end;

    select @WorkshiftCode = workshift_code from mes_org where record_id = @WorkshopId;
    select top 1 @SpanId = record_id       
      from mes_workshift_span sp
     where  is_break = 0
       and cast((CONVERT(varchar(10),@ReqTime,120) + ' '+ sp.time_begin) as datetime) <= @ReqTime
	   and cast((CONVERT(varchar(10),@ReqTime,120) + ' '+ sp.time_end) as datetime) >= @ReqTime
       and sp.workshift_id in(
          select record_id from mes_workshift where shift_code = @WorkshiftCode
       )
      order by time_begin desc;

    if(@SpanId = -1)  begin
        set @RespData='2|1|3';
        set @RespData= @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
        set @RespData= @RespData + '|1|休息时间段，|0'; 
		set @RespData= @RespData + '|2|不可计数.|0'; 

        return;       
    end;

	set @IsLastWorkstation = 0;
	if(@WorkstationSeq =  (select max(seq) from mes_org L where L.parent_id  = @LineId))  -- 是否是最后一个产线计数的工位
	   set @IsLastWorkstation =1;

    -- --------------------------------------------------------------------------------------------------------   
    if (@ReqDataType in(2,9)) begin -- 通过按键进行不良品报工        
        if (@ReqDataType = 2) and (not @ReqData in('11','12')) 
        begin
            set @RespData='2|1|4';
            set @RespData= @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
            set @RespData= @RespData + '|1|操作错误:|0';
            set @RespData= @RespData + '|2|报不良按[确定]键.|0';
            set @RespData= @RespData + '|3|清零按[取消]键.|0';

            return;
        end;
    end;
    -- --------------------------------------------------------------------------------------------------------

    if (@ReqDataType = 2) and (@ReqData = '12') begin
		update mes_workstation_output
		      set last_count_good = 0,
				  last_count_bad = 0
		   where workstation_code = @WorkstationCode 
		    and workorder_no=@WorkorderNo;

        set @RespData='2|1|2';
        set @RespData= @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
        set @RespData= @RespData + '|1|统计数据已清零|0';     

        return;
    end ;

    -- --------------------------------------------------------------------------------------------------------

    set @RecordType = 0 ; -- 默认为良品报工
    if (@DefectReportMethod = @ReqDataType) begin
        set @RecordType = 1;  -- 不良品报工
    end;      
    
    set @ProductDate = cast(CONVERT(varchar(10),@ReqTime,120) as datetime);

    insert into mes_workorder_actual(workorder_no,part_no,workstation_code,qty,record_type,defect_code,report_time) 
         values(@WorkorderNo,@PartNo,@WorkstationCode,1,@RecordType,@DefectCode,@ReqTime);

    if @RecordType =  0 begin
        select @QtyGood = 1,@QtyBad = 0;
    end else begin
        select @QtyGood = 0,@QtyBad = 1;
    end;

    update mes_active_workorder
       set gid = @GID,did = @DID,last_update_time = GETDATE(), update_status = 1
    where workorder_no = @WorkorderNo;

    begin tran;
        select @lock = count(*) from mes_active_workorder with(rowlock,xlock)
           where workorder_no = @WorkorderNo;
                
        if not exists(
            select * from mes_line_product_summary_datespan
               where line_no = @LineNo and workorder_no = @WorkorderNo 
                 and product_date = @ProductDate and span_id = @SpanId
        )begin			
            insert into mes_line_product_summary_datespan(line_no,workorder_no,part_no,product_date,span_id,qty_good,qty_bad)
                 values(@LineNo,@WorkorderNo,@PartNo,@ProductDate,@SpanId,(case when @IsLastWorkstation = 1 then @QtyGood else 0 end),@QtyBad);

        end else 
            update mes_line_product_summary_datespan
                set qty_good = qty_good + (case when @IsLastWorkstation = 1 then @QtyGood else 0 end),
                   qty_bad = qty_bad + @QtyBad
            where line_no = @LineNo and workorder_no = @WorkorderNo 
              and product_date = @ProductDate and span_id = @SpanId;
        
        update  mes_workorder
           set qty_good = qty_good + (case when @IsLastWorkstation = 1 then @QtyGood else 0 end),
               qty_bad  = qty_bad + @QtyBad
         where order_no = @WorkorderNo;

        update mes_active_workorder
          set last_update_time = GETDATE(), update_status = 2
        where workorder_no = @WorkorderNo;

		if not exists(select * from mes_workstation_output where workstation_code = @WorkstationCode and workorder_no=@WorkorderNo)
		   insert into mes_workstation_output(workstation_code,workorder_no,qty_good,qty_bad,last_count_good,last_count_bad) 
		                               values(@WorkstationCode,@WorkorderNo,@QtyGood,@QtyBad,@QtyGood,@QtyBad);
		else
		   update mes_workstation_output
		      set qty_good = qty_good + @QtyGood,
			      qty_bad = qty_bad + @QtyBad,
				  last_count_good = last_count_good + @QtyGood,
				  last_count_bad = last_count_bad +@QtyBad
		   where workstation_code = @WorkstationCode 
		     and workorder_no=@WorkorderNo;

		select @QtyGood = last_count_good,@QtyBad =last_count_bad 
		   from mes_workstation_output
		   where workstation_code = @WorkstationCode 
		     and workorder_no=@WorkorderNo;
    commit tran; 
	
	set @RespData='2|1|5';
	set @RespData= @RespData + '|210|0|128|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
	set @RespData= @RespData + '|1|已处理，请继续|0';
    set @RespData= @RespData + '|2|'+@WorkstationName+'|0'; 
	set @RespData= @RespData + '|3|良品:'+cast(@QtyGood as varchar)+'|0'; 
	set @RespData= @RespData + '|4|不良品:'+cast(@QtyBad as varchar)+'|0'; 	
end;