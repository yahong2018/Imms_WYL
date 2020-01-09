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

INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_05', 'SYS02_05', '生产班次', 'app.view.imms.timesheet.TimeSheet', 0, '',  'SYS02', '0xf0c9',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_01', 'SYS02_01', '组织结构', 'app.view.imms.org.Organization', 1, '',  'SYS02', '0xf0e8',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_02', 'SYS02_02', '操作员管理', 'app.view.imms.org.operator.Operator',2, '', 'SYS02', '0xf2be',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_03', 'SYS03_03', '生产计划', 'app.view.imms.mfc.workorder.Workorder', 3, '', 'SYS02', '0xf03a',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_06', 'SYS03_06', '订单实绩', 'app.view.imms.mfc.workstationProductSummary.WorkstationProductSummary', 4, '', 'SYS02', '0xf1ea',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_07', 'SYS03_07', '每日实绩', 'app.view.imms.mfc.lineProductSummaryDateSpan.LineProductSummaryDateSpan', 5, '', 'SYS02', '0xf1ea',0);
INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_04', 'SYS03_04', '报工流水', 'app.view.imms.mfc.workorderActual.WorkorderActual', 6, '', 'SYS02', '0xf0cb',0);

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'DELETE', '删除');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'KANBAN_WORKSHOP', '工场板');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'KANBAN_PLANT', '总场板');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_01', 'KANBAN_LINE', '产线板');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'INSERT', '新增');
INSERT INTO mes_program_privilege (progra  m_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_02', 'DELETE', '删除');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'DELETE', '删除');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'IMPORT', '导入');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'START', '开工');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'COMPLETE', '完工');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_04', 'RUN', '运行');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_05', 'RUN', '运行');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_05', 'INSERT', '新增');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_05', 'UPDATE', '修改');
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_05', 'DELETE', '删除');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_06', 'RUN', '运行');

INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_07', 'RUN', '运行');

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
    record_id                  bigint            identity(1,1),
    org_code                   varchar(20)       not null,
    org_name                   varchar(50)       not null,
    org_type                   varchar(20)       not null,
         
    parent_id                  bigint            not null default 0,
    gid                        int               not null default 0,
    did                        int               not null default 0,
         
    seq                        int               not null,
    defect_report_method       int               not null default 0,  -- 不良汇报方式:  3.按键报不良 9. 光感报不良     
    workshift_code             varchar(20)       null,

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
    part_name         varchar(50)      not null,

    qty_req           int              not null,
    qty_good          int              not null default 0,
    qty_bad           int              not null default 0,   
    uph               int              not null,
    worker_count      int              not null,

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

    span_id            bigint          not null,

    qty_good           int             not null,
    qty_bad            int             not null,

    primary key(record_id)
);

create table mes_workstation_output
(
    record_id          bigint         identity(1,1),
    workstation_code   varchar(20)    not null,
    workorder_no       varchar(20)    not null,
    qty_good           int            not null,
    qty_bad            int            not null,

    last_count_good    int            not null,
    last_count_bad     int            not null,

    primary key(record_id)
);

create table mes_workshift(
    record_id        bigint           identity(1,1),
    shift_code       varchar(20)      not null,
    shift_name       varchar(50)      not null,
    shift_status     int              not null,

    primary key(record_id)
);


create table mes_workshift_span
(
    record_id         bigint          identity(1,1),
    workshift_id      bigint          not null,
    seq               int             not null,    
    time_begin        varchar(5)      not null,
    time_end          varchar(5)      not null,
    is_break          int             not null,
    delay_time        int             not null  default 0,

    primary key(record_id)
);

create table mes_workstation_product_summary
(
    record_id           bigint     identity(1,1)   not null,
    order_no            varchar(20)                not null,
    part_no             varchar(20)                not null,
    line_no             varchar(20)                not null,
    workstation_code    varchar(20)                not null,
    qty_good            int                        not null,
    qty_bad             int                        not null,

    primary key(record_id)
);

