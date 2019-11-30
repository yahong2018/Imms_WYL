/* *********************************************************************************************
*       初始化组织结构
***********************************************************************************************/
declare @workshop_id bigint, @line_id bigint;
set @line_id=0;
set @workshop_id = 0;

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01','车间1','ORG_WORK_SHOP',0,0,0,0,0);
set @workshop_id = SCOPE_IDENTITY();
-- --------------------------------------------------------------------------------------------

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L01','1#','ORG_WORK_LINE',@workshop_id,0,0,0,0);
set @line_id = SCOPE_IDENTITY();

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L01S01','1#测试','ORG_WORK_STATION',@line_id,1,1,1,9);  -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L01S02','1#外观','ORG_WORK_STATION',@line_id,1,2,2,9); -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L01S03','1#包装','ORG_WORK_STATION',@line_id,1,3,3,3);  -- 按键报不良

-- --------------------------------------------------------------------------------------------
insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L02','2#','ORG_WORK_LINE',@workshop_id,0,0,0,0);
set @line_id = SCOPE_IDENTITY();

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L02S01','2#测试','ORG_WORK_STATION',@line_id,1,4,1,9);  -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L02S02','2#外观','ORG_WORK_STATION',@line_id,1,5,2,9); -- 光感报不良

insert into mes_org(org_code,org_name,org_type,parent_id,gid,did,seq,defect_report_type)
    values('W01L02S03','2#包装','ORG_WORK_STATION',@line_id,1,6,3,3);  -- 按键报不良

/* *********************************************************************************************
*       初始化工单
***********************************************************************************************/
insert into mes_workorder(order_no,order_status,line_no,customer_no,part_no,qty_req,qty_good,qty_bad,time_start_plan,time_end_plan)
       values('112961-1-1',0,'W01L01','GENIE','AF89-OHD-02R',2000,0,0,'2019/11/30','2019/11/30');

insert into mes_active_workorder(line_no,workorder_no,part_no,update_status)
       values('W01L01','112961-1-1','AF89-OHD-02R',0); 

/* *********************************************************************************************
*       品质代码
***********************************************************************************************/             
insert into mes_defect(defect_code,defect_name)values('1','开裂');
insert into mes_defect(defect_code,defect_name)values('2','假焊');




