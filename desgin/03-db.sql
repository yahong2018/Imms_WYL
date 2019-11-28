create table mes_org
(
    record_id         bigint          identity(1,1),
    org_code          varchar(20)       not null,
    org_name          varchar(50)       not null,
    org_type          varchar(20)       not null,

    parent_id         int               not null default 0,
    gid               int               not null default 0,
    did               int               not null default 0,

    seq               int               not null,

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
    order_status      int              not null,  -- 0. 已计划   1.生产中     254.已取消     255.已完工   
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
    order_no          varchar(20)      not null,
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


