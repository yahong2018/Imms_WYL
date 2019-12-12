/* *********************************************************************************************
*       初始化组织结构
***********************************************************************************************/
declare @workshop_id bigint, @line_id bigint;
set @line_id=0;
set @workshop_id = 0;

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('W01','车间1','ORG_WORK_SHOP',0,0,0,0,0);
set @workshop_id = SCOPE_IDENTITY();
-- --------------------------------------------------------------------------------------------

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-1','1#','ORG_WORK_LINE',@workshop_id,0,0,0,0);
set @line_id = SCOPE_IDENTITY();

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-1.S01','1#测试','ORG_WORK_STATION',@line_id,1,1,1,9);  -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-1.S02','1#外观','ORG_WORK_STATION',@line_id,1,2,2,9); -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-1.S03','1#包装','ORG_WORK_STATION',@line_id,1,3,3,3);  -- 按键报不良

-- --------------------------------------------------------------------------------------------
insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-2','2#','ORG_WORK_LINE',@workshop_id,0,0,0,0);
set @line_id = SCOPE_IDENTITY();

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-2.S01','2#测试','ORG_WORK_STATION',@line_id,1,4,1,9);  -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-2.S02','2#外观','ORG_WORK_STATION',@line_id,1,5,2,9); -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_method)
    values('A301-2.S03','2#包装','ORG_WORK_STATION',@line_id,1,6,3,3);  -- 按键报不良

/* *********************************************************************************************
*       初始化工单
***********************************************************************************************/
insert into mes_workorder(order_no,order_status,line_no,customer_no,part_no,qty_req,qty_good,qty_bad,time_start_plan,time_end_plan)
       values('112961-1-1',0,'A301-1','GENIE','AF89-OHD-02R',2000,0,0,'2019/11/30','2019/11/30');

insert into mes_active_workorder(line_no,workorder_no,part_no,update_status)
       values('A301-1','112961-1-1','AF89-OHD-02R',0); 

/* *********************************************************************************************
*       品质代码
***********************************************************************************************/             
insert into mes_defect(defect_code,defect_name)values('1','开裂');
insert into mes_defect(defect_code,defect_name)values('2','假焊');

/* *********************************************************************************************
*       操作员
***********************************************************************************************/             
insert into mes_operator(org_code,emp_id,emp_name,title,pic,seq)
   values('A301-1','C00001','张秀丽','生产组长','upload/operators/W01/A301-1/C00001_张秀丽.png',1);

insert into mes_operator(org_code,emp_id,emp_name,title,pic,seq)
   values('A301-1','C00002','李可先','工程师','upload/operators/W01/A301-1/C00002_李可先.png',2);

insert into mes_operator(org_code,emp_id,emp_name,title,pic,seq)
   values('A301-1','C00003','胡玲','高级组长','upload/operators/W01/A301-1/C00003_胡玲.png',3);   

insert into mes_operator(org_code,emp_id,emp_name,title,pic,seq)
   values('A301-1','C00004','莫房昌','品质工程师','upload/operators/W01/A301-1/C00004_莫房昌.png',4);    

/* *********************************************************************************************
*       报工
***********************************************************************************************/             
declare @DataGatherTime datetime;
declare @RespData varchar(200);

set @DataGatherTime = GETDATE();
-- exec MES_ProcessDeviceData 1,3,2,'11',@DataGatherTime,@RespData output;

exec MES_ProcessDeviceData 1,2,9,'11',@DataGatherTime,@RespData output;

select @RespData;
  


