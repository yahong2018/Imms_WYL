drop table mes_system_user;
drop table mes_system_role;
drop table mes_role_user;
drop table mes_role_privilege;
drop table mes_system_program;
drop table mes_program_privilege;

CREATE TABLE mes_system_user
(
    record_id                 BIGINT  identity(1,1)      NOT NULL,
    user_code                 VARCHAR(20)                NOT NULL,
    user_name                 VARCHAR(50)                NOT NULL,
    pwd                       VARCHAR(50)                NOT NULL,
    user_status               TINYINT                    NOT NULL,
    email                     VARCHAR(255)               NOT NULL,
    is_online                 BIT                        NOT NULL DEFAULT 0, -- 0.离线  1.在线
    last_login_time           DATETIME                   NULL ,              -- 最后登录时间

    PRIMARY KEY (record_id)        
);


CREATE TABLE mes_system_role
(
    record_id                  BIGINT identity(1,1)    NOT NULL,
    role_code                  VARCHAR(20)              NOT NULL,
    role_name                  VARCHAR(50)              NOT NULL,

    PRIMARY KEY (record_id)
);


CREATE TABLE mes_role_user
(
    record_id                 BIGINT identity(1,1)     NOT NULL,
    role_id                   BIGINT                    NOT NULL,
    user_id                   BIGINT                    NOT NULL,

    PRIMARY KEY (record_id)
);


CREATE TABLE mes_role_privilege
(
    record_id                 BIGINT identity(1,1)      NOT NULL,
    program_privilege_id      BIGINT                     NOT NULL,
    role_id                   BIGINT                     NOT NULL,
    program_id                VARCHAR(50)                NOT NULL,
    privilege_code            VARCHAR(50)                NOT NULL,

    PRIMARY KEY (record_id)
);


CREATE TABLE mes_system_program
(
    record_id                  VARCHAR(50)                NOT NULL,
    program_code               VARCHAR(50)                NOT NULL,
    program_name               VARCHAR(120)               NOT NULL,
    url                        VARCHAR(255)               NOT NULL,
    glyph                      VARCHAR(100)               NULL,
    show_order                 INT                        NOT NULL,
    parameters                 VARCHAR(255)               NOT NULL,
    parent_id                  VARCHAR(50)                NOT NULL,
    program_status             int                        not null,

    PRIMARY KEY (record_id)
);

CREATE TABLE mes_program_privilege
(
    record_id                  BIGINT identity(1,1)      NOT NULL,
    program_id                 VARCHAR(50)                   NOT NULL,
    privilege_code             VARCHAR(50)                NOT NULL,
    privilege_name             VARCHAR(120)               NOT NULL,

    PRIMARY KEY (record_id)
);
truncate table mes_system_user;
truncate table mes_system_role;
truncate table mes_role_user;
truncate table mes_system_program;
truncate table mes_program_privilege;
truncate table mes_role_privilege;

declare @user_id bigint,@role_id bigint;

INSERT INTO mes_system_user (user_code, user_name, pwd, user_status, email, last_login_time,is_online) VALUES ('C00001', '刘永红', 'e10adc3949ba59abbe56e057f20f883e', 0, 'liuyonghong@zhxh.com', NULL,0);
set @user_id = SCOPE_IDENTITY();

INSERT INTO mes_system_role (role_code, role_name) VALUES ('admin', '系统管理员');
set @role_id=SCOPE_IDENTITY ();

INSERT INTO mes_role_user (role_id, user_id) VALUES (@role_id, @user_id);

INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters,  parent_id, glyph,program_status) VALUES ('SYS01','SYS01', '系统管理', '', 0, '', '', '0xf013',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters,  parent_id, glyph,program_status) VALUES ('SYS01_01','SYS01_01', '用户管理', 'app.view.admin.systemUser.SystemUser', 0, '', 'SYS01', '0xf007',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status)  VALUES ('SYS01_02','SYS01_02', '角色管理', 'app.view.admin.systemRole.SystemRole', 1, '', 'SYS01', '0xf233',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status)  VALUES ('SYS01_03','SYS01_03', '系统参数', 'app.view.admin.systemParameter.SystemParameter', 2, '','SYS01', '0xf085',0);

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01', 'RUN', '系统运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'RUN', '系统运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'DELETE', '删除');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'STOP_USER', '暂停账户');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'START_USER', '启用账户');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'RESET_PASSWORD', '重设密码');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_01', 'ASSIGN_ROLE', '授权');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_02', 'RUN', '系统运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_02', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_02', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_02', 'DELETE', '删除');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_02', 'ASSIGN_ROLE', '授权');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_03', 'RUN', '系统运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS01_03', 'UPDATE', '修改');

-- ----------------------------------------------------------------------------------------------------------------------

INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status)VALUES ('SYS02','SYS02', '看板系统', '', 1, '', '', '0xf0ae',0);
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02', 'RUN', '运行');

INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_01', 'SYS02_01', '组织结构', 'app.view.imms.org.Organization', 0, '',  'SYS02', '0xf0e8',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_02', 'SYS02_02', '操作员管理', 'app.view.imms.org.operator.Operator',1, '', 'SYS02', '0xf2be',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_03', 'SYS03_03', '生产计划', 'app.view.imms.mfc.workorder.Workorder', 2, '', 'SYS02', '0xf03a',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_04', 'SYS03_04', '生产实绩', 'app.view.imms.mfc.workorderActual.WorkorderActual', 3, '', 'SYS02', '0xf0cb',0);

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'DELETE', '删除');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'KANBAN_WORKSHOP', '工场板');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'KANBAN_PLANT', '总场板');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'KANBAN_LINE', '产线板');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'DELETE', '删除');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'DELETE', '删除');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'IMPORT', '导入');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'START', '开工');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_04', 'RUN', '运行');

-- --------------------------------------------------------------------------------------------------------------------

set @role_id = 1;
INSERT INTO mes_role_privilege (role_id, program_privilege_id, program_id, privilege_code)
    SELECT
        @role_id,
        prv.record_id,
        prv.program_id,
        prv.privilege_code
    FROM mes_program_privilege prv
    WHERE record_id NOT IN (
        SELECT program_privilege_id  FROM mes_role_privilege
    );

-- --------------------------------------------------------------------------------------------------------------------

create table mes_org
(
    record_id                  bigint          identity(1,1),
    org_code                   varchar(20)       not null,
    org_name                   varchar(50)       not null,
    org_type                   varchar(20)       not null,
         
    parent_id                  bigint            not null default 0,
    gid                        int               not null default 0,
    did                        int               not null default 0,
         
    seq                        int               not null,
    defect_report_method       int               not null default 0,  -- 不良汇报方式:  3.按键报不良 9. 光感报不良     

    primary key(record_id)
);

create table mes_operator
(
    record_id         bigint         identity(1,1),
    org_code          varchar(20)      not null,
    emp_id            varchar(20)      not null,
    emp_name          varchar(50)      not null,
    title             varchar(20)      not null,
    pic               varchar(255)     not null,
    seq               int              not null,

    primary key(record_id)
);

create table mes_defect
(
    record_id         bigint         identity(1,1),
    defect_code       varchar(20)     not null,
    defect_name       varchar(50)     not null,

    primary key(record_id)
);

create table mes_workorder
(
    record_id         bigint         identity(1,1),
    order_no          varchar(20)      not null,
    order_status      int              not null,    -- 0. 已计划       1.生产中     254.已取消     255.已完工   
                                                    -- 注意：一条产线只能有一个工单是处于"生产中"的状态

    line_no           varchar(20)      not null,
    customer_no       varchar(50)      not null,
    part_no           varchar(50)      not null,

    qty_req           int              not null,
    qty_good          int              not null default 0,
    qty_bad           int              not null default 0,

    time_start_plan   datetime         not null,
    time_end_plan     datetime         not null,

    time_start_actual datetime         null,
    time_end_actual   datetime         null,

    primary key(record_id)
);

create table mes_active_workorder
(
    record_id         bigint         identity(1,1),
    line_no           varchar(20)      not null,
    workorder_no      varchar(20)      not null,
    part_no           varchar(50)      not null,

    last_update_time  datetime         null,
    gid               int              null,
    did               int              null,
    update_status     int              not null, -- 0.未更新    1. 开始更新     2.更新完成

    primary key (record_id)
);

create table mes_workorder_actual
(
    record_id          bigint        identity(1,1),
    workorder_no       varchar(20)     not null,
    part_no            varchar(50)     not null,
    workstation_code   varchar(20)     not null,    
    qty                int             not null,
    record_type        int             not null,  -- 记录类型: 0.良品    1.不良品
    defect_code        varchar(20)     not null,  
    report_time        datetime        not null,

    primary key(record_id)
);

-- create table mes_line_product_summary_date
-- (
--     record_id          bigint        identity(1,1),
--     line_no            varchar(20)     not null,
--     workorder_no       varchar(20)     not null,
--     part_no            varchar(50)     not null,
--     product_date       datetime        not null,
        
--     -- qty_plan_date      int             not null,
--     -- qty_plan           int             not null,

--     qty_good           int             not null,
--     qty_bad            int             not null,

--     primary key(record_id)
-- );

create table mes_line_product_summary_datespan
(
    record_id          bigint        identity(1,1),
    line_no            varchar(20)     not null,
    workorder_no       varchar(20)     not null,
    part_no            varchar(50)     not null,
    product_date       datetime        not null,

    span_id            int             not null,

    qty_plan           int             not null,
    qty_good           int             not null,
    qty_bad            int             not null,

    primary key(record_id)
);

create procedure MES_Light(
    @GID             int,
    @DID             int,
    @Lamp            int
)as
begin
    declare @data varchar(200),@DeviceCmdID int;
    if @Lamp = 1 
       set @data = '2|0|1|230|0E0100110000220000320000000074';  -- 红灯
    else 
       set @data = '2|0|1|230|0E0100120000220000310000000074';  -- 绿灯
    
    exec GetNewDeviceCmdID @DeviceCmdID=@DeviceCmdID OUTPUT;
    insert into DeviceCmdList(DeviceCmdID,CmdType,GID,DID,CmdNumber,CmdContent,CmdMakeTime,RetryTimes)
                       values(@DeviceCmdID,0,@GID,@DID,28,@data,GETDATE(),3);
end;

create procedure MES_ProcessDeviceData 
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
    declare @DefectCode varchar(20),@WorkstationCode varchar(20), @WorkorderNo varchar(20),@LineNo varchar(20);
    declare @RecordType int,@SpanId int,@DefectReportMethod int,@ProductDate datetime,@PartNo varchar(50);
    declare @QtyGood int,@QtyBad int,@QtyPlan int,@lock int;

    select @RespData ='',@DefectCode='',@WorkstationCode='',@WorkorderNo='',@RecordType=-1,@SpanId=-1,
           @QtyGood = 0,@QtyBad = 0 ,@QtyPlan = 0;    

    if (@ReqDataType = 9) and (@ReqData<>'8203800000002AE3')
        return;
	   
	if (select count(*)  from mes_org w
    where w.org_type = 'ORG_WORK_STATION'
      and w.gid = @GID
      and w.did = @DID )>1 begin		
		set @RespData = '2|1|4'; 
        set @RespData = @RespData + '|210|0|129|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
		set @RespData = @RespData + '|1|组号:' + cast(@GID as varchar(10)) + ',机号:' + cast(@DID as varchar(10)) +'|0';
		set @RespData = @RespData + '|2|工位重复注册|0';			
		set @RespData = @RespData + '|3|请联系管理员|0';        

		return;
	end;

    select @WorkstationCode = org_code, @DefectReportMethod = defect_report_method
        from mes_org where gid = @GID and did = @DID;
    if(@WorkstationCode = '')begin
		set @RespData ='2|1|4';
		set @RespData = @RespData + '|210|0|129|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
        set @RespData = @RespData + '|1|组号:' + cast(@GID as varchar(10)) + ',机号:' + cast(@DID as varchar(10)) +'|0';
		set @RespData = @RespData + '|2|工位没有注册.|0';
        set @RespData = @RespData + '|3|请联系管理员|0';

        return;
    end;
    select top 1 @LineNo = L.org_code  from mes_org  L
      where L.record_id = (
        select parent_id from mes_org W where W.org_code = @WorkstationCode
    );
    select top 1 @WorkorderNo = workorder_no,@PartNo = part_no  from mes_active_workorder A
        where A.line_no = @LineNo;
    if @WorkorderNo = ''  begin
        set @RespData='2|1|2';
        set @RespData= @RespData + '|210|0|129|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
        set @RespData= @RespData + '|1|还没有启用工单.|0';         

        return;
    end;

    -- --------------------------------------------------------------------------------------------------------
    set @RecordType = 0 ; -- 默认为良品报工
    if (@DefectReportMethod = @ReqDataType) begin
        set @RecordType = 1;  -- 不良品报工
    end;
    
    if (@RecordType = 1)  and (@ReqDataType = 3 ) begin -- 通过按键进行不良品报工
        select @DefectCode = defect_code from mes_defect where defect_code = @ReqData;
        if  @DefectCode = '' begin
		    set @RespData='2|1|2';
			set @RespData= @RespData + '|210|0|129|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|0|3|50';
			set @RespData= @RespData + '|1|品质代码错误.|0';            

            return;
        end;
    end;
    -- --------------------------------------------------------------------------------------------------------
    set @SpanId = DATEPART(hh,@ReqTime);
    set @ProductDate = cast(CONVERT(varchar(10),@ReqTime,120) as datetime);   

    insert into mes_workorder_actual(workorder_no,part_no,workstation_code,qty,record_type,defect_code,report_time) 
         values(@WorkorderNo,@PartNo,@WorkstationCode,1,@RecordType,@DefectCode,@ReqTime);

    if @RecordType =  0 begin
        select @QtyGood = 1,@QtyBad = 0;
    end else begin
        select @QtyGood = 0,@QtyBad = 1;
    end;

    select @QtyPlan = qty_req from mes_workorder where line_no = @LineNo;
    set @QtyPlan = isnull(@QtyPlan,0);
    set @QtyPlan = @QtyPlan / 10;
    set @QtyPlan = 100;

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
        )
            insert into mes_line_product_summary_datespan(line_no,workorder_no,part_no,product_date,span_id,qty_plan,qty_good,qty_bad)
                 values(@LineNo,@WorkorderNo,@PartNo,@ProductDate,@SpanId,@QtyPlan, @QtyGood,@QtyBad);
        else 
            update mes_line_product_summary_datespan
                set qty_good = qty_good + @QtyGood,
                   qty_bad = qty_bad + @QtyBad
            where line_no = @LineNo and workorder_no = @WorkorderNo 
              and product_date = @ProductDate and span_id = @SpanId;
        
        update  mes_workorder
           set qty_good = qty_good + @QtyGood,
               qty_bad  = qty_bad + @QtyBad
         where order_no = @WorkorderNo;

        update mes_active_workorder
        set last_update_time = GETDATE(), update_status = 2
        where workorder_no = @WorkorderNo;         
    commit tran;    
end;


