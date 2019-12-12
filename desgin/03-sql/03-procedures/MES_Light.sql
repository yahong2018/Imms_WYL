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
