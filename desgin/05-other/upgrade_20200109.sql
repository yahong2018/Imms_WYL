INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_03', 'COMPLETE', '完工');
INSERT INTO mes_role_privilege (role_id, program_privilege_id, program_id, privilege_code)
    SELECT
        1,
        prv.record_id,
        prv.program_id,
        prv.privilege_code
    FROM mes_program_privilege prv
    WHERE record_id NOT IN (
        SELECT program_privilege_id  FROM mes_role_privilege
    );


INSERT INTO mes_system_program (record_id,program_code, program_name, url, show_order, parameters, parent_id, glyph,program_status) VALUES ('SYS02_07', 'SYS03_07', '每日实绩', 'app.view.imms.mfc.lineProductSummaryDateSpan.LineProductSummaryDateSpan', 5, '', 'SYS02', '0xf1ea',0);
INSERT INTO mes_program_privilege (program_id, privilege_code, privilege_name) VALUES ('SYS02_07', 'RUN', '运行');