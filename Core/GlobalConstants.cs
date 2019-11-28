using System;
using System.Collections.Generic;
using System.Text;
using Imms.Data;
using Imms.Security.Data.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Imms
{
    public static class GlobalConstants
    {
        //数据操作
        public const int DML_OPERATION_INSERT = 0;
        public const int DML_OPERATION_UPDATE = 10;
        public const int DML_OPERATION_DELETE = 100;
        public const int DML_OPERATION_LOAD = 127;

        //系统异常代码
        public const int EXCEPTION_CODE_NO_ERROR = 0; //无异常
        public const int EXCEPTION_CODE_DATA_ALREADY_EXISTS = 1;//数据已经存在
        public const int EXCEPTION_CODE_DATA_NOT_FOUND = 2;//数据没有找到
        public const int EXCEPTION_CODE_NOT_EXCEPTED_DATA = 3;//非预期数据
        public const int EXCEPTION_CODE_DATA_REPEATED = 4;  //数据重复
        public const int EXCEPTION_CODE_PARAMETER_INVALID = 5;  //参数错误
        public const int EXCEPTION_CODE_EXISTS_RELATED_ITEMS = 6; //存在有相关项，不能删除
        public const int EXCEPTION_CODE_CUSTOM = int.MaxValue; //业务逻辑自定义的异常

        //数据交换:规则
        public const string DATA_EXCHANGE_RULE__PRODUCITON_ORDER__APS_2_MES = "PRODUCITON_ORDER_APS_2_MES";
        public const string DATA_EXCHANGE_RULE__PRODUCITON_ORDER__CAD_2_MES = "PRODUCITON_ORDER_CAD_2_MES";
        public const string DATA_EXCHANGE_RULE__PRODUCITON_ORDER__GST_2_MES = "PRODUCITON_ORDER_GST_2_MES";
        //数据交换：系统ID
        public const int SYSTEM_ID_MES = 1;
        public const int SYSTEM_ID_APS = 2;
        public const int SYSTEM_ID_MCS = 3;
        public const int SYSTEM_ID_KANBAN = 4;

        //组织机构类型
        public const string TYPE_ORG_PLANT = "ORG_PLANT";   // 工厂
        public const string TYPE_ORG_WORK_SHOP = "ORG_WORK_SHOP"; //车间
        public const string TYPE_ORG_WORK_CENTER = "ORG_WORK_CENTER";  //工作中心
        public const string TYPE_ORG_WORK_LINE = "ORG_WORK_LINE";      // 生产线
        public const string TYPE_ORG_WORK_STATETION = "ORG_WORK_STATION";  //工作站

        //组织架构参数
        //public const string TYPE_ORG_PARAMETER_TYPE_WORK_STATION_TYPE = "WORK_STATION_TYPE";  //工位类型
        //public const string TYPE_ORG_PARAMETER_TYPE_STATUS_IS_ON_LINE = "STATUS_IS_ON_LINE";  //离线|在线
        //public const string TYPE_ORG_PARAMETER_TYPE_WORK_STATION_DEVICE_TYPE = "WORK_STATION_DEVICE_TYPE"; //工位的机器类型

        //组织架构参数的值
        public const string VALUE_ORG_PARAMETER_STATUS_ON_LINE = "OFF_LINE"; //离线
        public const string VALUE_ORG_PARAMETER_STATUS_OFF_LINE = "OFF_LINE"; //离线

        //编码类型        
        public const string TYPE_CODE_TYPE_EQUIPMENT_TYPE = "EQUIPMENT_TYPE";//设备类型      
        public const string TYPE_CODE_TYPE_MATERIAL_TYPE = "MATERIAL_TYPE";  //物料类型

        //树形编码
        public const string TYPE_CODE_TYPE_DEFECT = "TREE_CODE_TYPE_DEFECT";//缺陷代码


        public const string JWT_SECRET_STRING="ZHXH_IMMS_SECURITY_V_2.0"; 
        public const string JWT_ISSURER_URL="http://localhost:5200";
        public const string AUTHROIZATION_SESSION_KEY = "Authorization";

        //其他
        public static Imms.Data.IDbContextFactory DbContextFactory = null;
        public static Logger DefaultLogger = new Logger();

        public static void ModifyEntityStatus<T>(T item, DbContext dbContext) where T : class, IEntity
        {
            T old = dbContext.Set<T>().Find(item.RecordId);
            dbContext.Entry<T>(old).CurrentValues.SetValues(item);
        }

        //系统编码
        public static readonly string DEFAULT_CHARSET = System.Environment.GetEnvironmentVariable("DEFAULT_CHARSET", EnvironmentVariableTarget.User);
        public static readonly System.Text.Encoding DEFAULT_ENCODING;
        static GlobalConstants()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (string.IsNullOrEmpty(DEFAULT_CHARSET))
            {
                DEFAULT_ENCODING = System.Text.Encoding.Default;
            }
            else
            {
                DEFAULT_ENCODING = System.Text.Encoding.GetEncoding(DEFAULT_CHARSET);
            }

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        //
        //序列化
        //
        public static string ToJson(this object obj)
        {
            string result = JsonConvert.SerializeObject(obj);
            return result;
        }

        //
        //反序列化
        //
        public static T ToObject<T>(this string jsonStr) where T : class
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }

            T result = JsonConvert.DeserializeObject<T>(jsonStr);

            return result;
        }

        //
        //反序列化
        //
        public static object ToObject(this string jsonStr, Type type)
        {
            object result = JsonConvert.DeserializeObject(jsonStr, type);

            return result;
        }

        //
        //从文件反序列化
        //
        public static T LoadBeanFromFile<T>(this string fileName)
        {
            string gstJson = System.IO.File.ReadAllText(fileName);
            T result = JsonConvert.DeserializeObject<T>(gstJson);
            return result;
        }

        public static GetCurrentUserHandler GetCurrentUserDelegate;

        public static SystemUser GetCurrentUser()
        {
            if (GetCurrentUserDelegate != null)
            {
                return GetCurrentUserDelegate();
            }
            return null;
        }
    }

    public delegate SystemUser GetCurrentUserHandler();
}