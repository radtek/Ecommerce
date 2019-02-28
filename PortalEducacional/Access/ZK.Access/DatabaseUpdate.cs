/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	internal class DatabaseUpdate
	{
		public static AttParamBll attParamBll = new AttParamBll(MainForm._ia);

		public static AttParam attParam = null;

		private static List<string> WiegandDefaultList = new List<string>();

		public static void intWiegandDefault()
		{
			DatabaseUpdate.WiegandDefaultList.Clear();
			DatabaseUpdate.WiegandDefaultList.Add("SRBOn");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand26");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand26a");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand34");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand34a");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand36");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand37");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand37a");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand50");
			DatabaseUpdate.WiegandDefaultList.Add("Wiegand66");
		}

		public static void CheckAndUpdateDB()
		{
			try
			{
				AccCommon.IsECardTong = DatabaseUpdate.IsECardTongSign();
				int num = DatabaseUpdate.GetLastVersion();
				if (num >= 0)
				{
					if (num < 1)
					{
						WaitForm instance = WaitForm.Instance;
						string info = ShowMsgInfos.GetInfo("SUpdateDb", "数据库正在升级，请稍候...") + "\r\n";
						instance.ShowEx(info);
						Application.DoEvents();
						if (AppSite.Instance.DataType == DataType.Access)
						{
							try
							{
								DbHelperOleDb.ExecuteSql("alter table acc_door add reader_io_state int default 0 ");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("alter table acc_door add open_door_delay int default 15, door_normal_open bit,enable_normal_open bit,disenable_normal_open bit");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("alter table acc_levelset_door_group add accdoor_no_exp int default 0,accdoor_device_id int default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("alter table acc_levelset_door_group add level_timeseg_id int default 0");
							}
							catch
							{
							}
							try
							{
								string sQLString = "UPDATE   acc_levelset_door_group a,(select id,device_id, door_no as expsum from acc_door ) b,(select id, level_timeseg_id from acc_levelset) c SET  a.accdoor_no_exp = b.expsum, a.accdoor_device_id = b.device_id , a.level_timeseg_id = c.level_timeseg_id where a.accdoor_id = b.id and a.acclevelset_id = c.id ";
								DbHelperOleDb.ExecuteSql(sQLString);
							}
							catch
							{
							}
							try
							{
								string sQLString2 = "select  acclevelset_id ,accdoor_device_id,accdoor_no_exp from acc_levelset_door_group";
								DataSet dataSet = DbHelperOleDb.Query(sQLString2);
								DataTable dataTable = dataSet.Tables[0];
								List<string> list = new List<string>();
								for (int i = 0; i < dataTable.Rows.Count; i++)
								{
									instance.ShowProgress(100 * i / dataTable.Rows.Count);
									int num2 = int.Parse(dataTable.Rows[i]["acclevelset_id"].ToString());
									int num3 = int.Parse(dataTable.Rows[i]["accdoor_device_id"].ToString());
									int num4 = 0;
									for (int j = 0; j < dataTable.Rows.Count; j++)
									{
										if (num2 == int.Parse(dataTable.Rows[j]["acclevelset_id"].ToString()) && num3 == int.Parse(dataTable.Rows[j]["accdoor_device_id"].ToString()))
										{
											num4 += 1 << int.Parse(dataTable.Rows[j]["accdoor_no_exp"].ToString()) - 1;
										}
									}
									string item = num3.ToString() + "_" + num3.ToString();
									if (!list.Contains(item))
									{
										string sQLString3 = "update acc_levelset_door_group  set accdoor_no_exp =" + num4 + " where acclevelset_id = " + num2 + " and accdoor_device_id = " + num3;
										try
										{
											DbHelperOleDb.ExecuteSql(sQLString3);
											list.Add(item);
										}
										catch
										{
										}
									}
								}
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("alter table Machines add SimpleEventType smallint default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("CREATE TABLE FingerVein(FVID AUTOINCREMENT , UserID int default 0, FingerID int,Template image null, [Size] int default 0, DuressFlag int, UserCode varchar(50),CONSTRAINT FingerVein_PK PRIMARY KEY(FVID))");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("alter table Machines add FvFunOn Int default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("Update USERINFO set Gender = 'M' where trim(Gender) = '0' ");
								DbHelperOleDb.ExecuteSql("Update USERINFO set Gender = 'F' where rim(Gender) = '1' ");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("alter table Machines add fvcount SmallInt default 0");
							}
							catch
							{
							}
						}
						else if (AppSite.Instance.DataType == DataType.SqlServer)
						{
							try
							{
								DbHelperSQL.ExecuteSql("alter table acc_door add reader_io_state int default 0 ");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("alter table acc_door add open_door_delay int default 15, door_normal_open bit,enable_normal_open bit,disenable_normal_open bit");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("alter table acc_levelset_door_group add  accdoor_no_exp int default 0,accdoor_device_id int default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("alter table acc_levelset_door_group add level_timeseg_id int default 0");
							}
							catch
							{
							}
							try
							{
								string sQLString4 = "UPDATE a SET  a.accdoor_no_exp = sum(2^(b.accdoor_no-1)), a.accdoor_device_id = b.device_id,a.level_timeseg_id = c.level_timeseg_id from acc_levelset_door_group a , acc_door b , acc_levelset c  where a.accdoor_id = b.id and a.acclevelset_id = c.id group by b.device_id  ";
								DbHelperSQL.ExecuteSql(sQLString4);
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("alter table Machines add SimpleEventType int default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("CREATE TABLE FingerVein (FVID int identity(1,1) not null primary key, UserID int default 0,FingerID int,Template image null, Size int default 0, DuressFlag int, UserCode varchar(50)) ");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("alter table Machines add FvFunOn int default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("Update  set Gender = 'M' From USERINFO where rim(Gender) = '0'");
								DbHelperSQL.ExecuteSql("Update  set Gender = 'F' From USERINFO where rim(Gender) = '1'");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("alter table Machines add fvcount SmallInt default 0");
							}
							catch
							{
							}
						}
						num = 1;
						instance.ShowProgress(100);
						info = ShowMsgInfos.GetInfo("SUpdateDbFinish", "数据库升级完成") + "\r\n";
						instance.ShowEx(info);
						instance.HideEx(false);
					}
					if (num < 2)
					{
						if (AppSite.Instance.DataType == DataType.Access)
						{
							try
							{
								DbHelperOleDb.ExecuteSql("alter table Machines add fvcount SmallInt default 0");
							}
							catch
							{
							}
						}
						else if (AppSite.Instance.DataType == DataType.SqlServer)
						{
							try
							{
								DbHelperSQL.ExecuteSql("alter table Machines add fvcount SmallInt default 0");
							}
							catch
							{
							}
						}
						num = 2;
					}
					if (num < 3)
					{
						DatabaseUpdate.intWiegandDefault();
						List<string> list2 = new List<string>();
						List<string> list3 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
						{
							DataSet dataSet2 = DbHelperOleDb.Query("select wiegand_name from acc_wiegandfmt");
							if (dataSet2 != null && dataSet2.Tables.Count > 0)
							{
								DataTable dataTable2 = dataSet2.Tables[0];
								foreach (DataRow row in dataTable2.Rows)
								{
									list3.Add(row["wiegand_name"].ToString().ToLower());
								}
							}
							list2.Add("alter table acc_door add wiegandInType SmallInt default 1,wiegandOutType SmallInt default 1,wiegand_fmt_out_id SmallInt default 0");
							if (!list3.Contains("AutoMatchWiegandFmt".ToLower()))
							{
								list2.Add("insert into acc_wiegandfmt(status,wiegand_name) values(1,'AutoMatchWiegandFmt')");
							}
							for (int l = 0; l < DatabaseUpdate.WiegandDefaultList.Count; l++)
							{
								if (!list3.Contains(DatabaseUpdate.WiegandDefaultList[l].ToLower()))
								{
									list2.Add($"insert into acc_wiegandfmt(status,wiegand_name) values(2,'{DatabaseUpdate.WiegandDefaultList[l]}')");
								}
							}
							break;
						}
						case DataType.SqlServer:
						{
							DataSet dataSet2 = DbHelperSQL.Query("select wiegand_name from acc_wiegandfmt");
							if (dataSet2 != null && dataSet2.Tables.Count > 0)
							{
								DataTable dataTable2 = dataSet2.Tables[0];
								foreach (DataRow row2 in dataTable2.Rows)
								{
									list3.Add(row2["wiegand_name"].ToString().ToLower());
								}
							}
							list2.Add("alter table acc_door add wiegandInType SmallInt default 1,wiegandOutType SmallInt default 1,wiegand_fmt_out_id SmallInt default 0");
							if (!list3.Contains("AutoMatchWiegandFmt".ToLower()))
							{
								list2.Add("insert into acc_wiegandfmt(status,wiegand_name) values(1,'AutoMatchWiegandFmt')");
							}
							for (int k = 0; k < DatabaseUpdate.WiegandDefaultList.Count; k++)
							{
								if (!list3.Contains(DatabaseUpdate.WiegandDefaultList[k].ToLower()))
								{
									list2.Add($"insert into acc_wiegandfmt(status,wiegand_name) values(2,'{DatabaseUpdate.WiegandDefaultList[k]}')");
								}
							}
							break;
						}
						}
						for (int m = 0; m < list2.Count; m++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[m]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[m]);
									break;
								}
							}
							catch (Exception ex)
							{
								SysLogServer.WriteLog("Error On Update DataBase to 3 version：" + ex.Message, true);
							}
						}
					}
					if (num < 4)
					{
						try
						{
							DatabaseUpdate.intWiegandDefault();
							if (AppSite.Instance.DataType == DataType.Access)
							{
								DbHelperOleDb.ExecuteSql("alter table Machines add deviceOption image null");
							}
							else if (AppSite.Instance.DataType == DataType.SqlServer)
							{
								DbHelperSQL.ExecuteSql("alter table Machines add deviceOption image null");
							}
						}
						catch
						{
						}
						num = 4;
					}
					if (num < 5)
					{
						DatabaseUpdate.intWiegandDefault();
						if (AppSite.Instance.DataType == DataType.Access)
						{
							try
							{
								DbHelperOleDb.ExecuteSql("alter table acc_door add SRBOn SmallInt default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperOleDb.ExecuteSql("truncate table acc_wiegandfmt");
								DbHelperOleDb.ExecuteSql("insert into acc_wiegandfmt(status,wiegand_name) values(1,'AutoMatchWiegandFmt')");
								for (int n = 0; n < DatabaseUpdate.WiegandDefaultList.Count; n++)
								{
									string empty = string.Empty;
									empty = $"insert into acc_wiegandfmt(status,wiegand_name) values(2,'{DatabaseUpdate.WiegandDefaultList[n]}')";
									DbHelperOleDb.ExecuteSql(empty);
								}
							}
							catch
							{
							}
						}
						else if (AppSite.Instance.DataType == DataType.SqlServer)
						{
							try
							{
								DbHelperSQL.ExecuteSql("alter table acc_door add SRBOn SmallInt default 0");
							}
							catch
							{
							}
							try
							{
								DbHelperSQL.ExecuteSql("delete from acc_wiegandfmt");
								DbHelperSQL.ExecuteSql("insert into acc_wiegandfmt(status,wiegand_name) values(1,'AutoMatchWiegandFmt')");
								for (int num5 = 0; num5 < DatabaseUpdate.WiegandDefaultList.Count; num5++)
								{
									string empty2 = string.Empty;
									empty2 = $"insert into acc_wiegandfmt(status,wiegand_name) values(2,'{DatabaseUpdate.WiegandDefaultList[num5]}')";
									DbHelperSQL.ExecuteSql(empty2);
								}
							}
							catch
							{
							}
						}
						num = 5;
					}
					if (num < 6)
					{
						try
						{
							if (AppSite.Instance.DataType == DataType.Access)
							{
								DbHelperOleDb.ExecuteSql("alter table userinfo add OfflineBeginDate datetime,OfflineEndDate datetime");
								DbHelperOleDb.ExecuteSql("create table OfflinePermitGroups(id autoincrement primary key,GroupName varchar(24),BeginDate datetime,EndDate datetime)");
								DbHelperOleDb.ExecuteSql("create table OfflinePermitUsers(id autoincrement primary key,GroupId integer,Pin varchar(24),ChkPwd bit,ChkFp bit)");
								DbHelperOleDb.ExecuteSql("create table OfflinePermitDoors(id autoincrement primary key,GroupId integer,DeviceId integer,DoorNo integer)");
								DbHelperOleDb.ExecuteSql("create table LossCard(id autoincrement primary key,Pin varchar(20),CardNo varchar(50),LossDate datetime)");
								DbHelperOleDb.ExecuteSql("create table TmpPermitGroups(id autoincrement primary key,GroupName varchar(24),BeginDate datetime,EndDate datetime)");
								DbHelperOleDb.ExecuteSql("create table TmpPermitUsers(id autoincrement primary key,GroupId integer,CertifiNo varchar(50),UserName varchar(24),Gender varchar(2),CardNo varchar(20),OfflineBeginDate datetime,OfflineEndDate datetime)");
								DbHelperOleDb.ExecuteSql("create table TmpPermitDoors(id autoincrement primary key,GroupId integer,DeviceId integer,DoorNo integer)");
								DbHelperOleDb.ExecuteSql("create table ParamSet(ParamName varchar(20) primary key,ParamValue varchar(100))");
							}
							else if (AppSite.Instance.DataType == DataType.SqlServer)
							{
								DbHelperSQL.ExecuteSql("alter table userinfo add OfflineBeginDate datetime,OfflineEndDate datetime");
								DbHelperSQL.ExecuteSql("create table OfflinePermitGroups(id int identity(1,1) not null primary key,GroupName nvarchar(24),BeginDate datetime,EndDate datetime)");
								DbHelperSQL.ExecuteSql("create table OfflinePermitUsers(id int identity(1,1) not null primary key,GroupId int,Pin nvarchar(24),ChkPwd bit,ChkFp bit)");
								DbHelperSQL.ExecuteSql("create table OfflinePermitDoors(id int identity(1,1) not null primary key,GroupId int,DeviceId int,DoorNo int)");
								DbHelperSQL.ExecuteSql("create table LossCard(id int identity(1,1) not null primary key,Pin nvarchar(20),CardNo nvarchar(50),LossDate datetime)");
								DbHelperSQL.ExecuteSql("create table TmpPermitGroups(id int identity(1,1) not null primary key,GroupName nvarchar(24),BeginDate datetime,EndDate datetime)");
								DbHelperSQL.ExecuteSql("create table TmpPermitUsers(id int identity(1,1) not null primary key,GroupId int,CertifiNo nvarchar(50),UserName nvarchar(24),Gender nvarchar(2),CardNo nvarchar(20),OfflineBeginDate datetime,OfflineEndDate datetime)");
								DbHelperSQL.ExecuteSql("create table TmpPermitDoors(id int identity(1,1) not null primary key,GroupId int,DeviceId int,DoorNo int)");
								DbHelperSQL.ExecuteSql("create table ParamSet(ParamName nvarchar(20) not null primary key,ParamValue nvarchar(100))");
							}
						}
						catch
						{
						}
						num = 6;
					}
					if (num < 7)
					{
						try
						{
							if (AppSite.Instance.DataType == DataType.Access)
							{
								DbHelperOleDb.ExecuteSql("alter table userinfo add carNo varchar(50),carType varchar(50),carBrand varchar(50),carColor varchar(50)");
							}
							else if (AppSite.Instance.DataType == DataType.SqlServer)
							{
								DbHelperSQL.ExecuteSql("alter table userinfo add carNo nvarchar(50),carType nvarchar(50),carBrand nvarchar(50),carColor nvarchar(50)");
							}
						}
						catch
						{
						}
						num = 7;
					}
					if (num < 8)
					{
						try
						{
							if (AppSite.Instance.DataType == DataType.Access)
							{
								DbHelperOleDb.ExecuteSql("CREATE TABLE acc_reader (id autoincrement primary key,door_id long,reader_no integer,reader_name varchar(50),reader_state integer);");
								DbHelperOleDb.ExecuteSql("CREATE TABLE acc_auxiliary(id autoincrement primary key,device_id long,aux_no integer,printer_number varchar(50),aux_name varchar(50),aux_state integer);");
							}
							else if (AppSite.Instance.DataType == DataType.SqlServer)
							{
								DbHelperSQL.ExecuteSql("CREATE TABLE [acc_reader]([id] [bigint] IDENTITY(1,1) NOT NULL,[door_id] [int] NULL,[reader_no] [int] NULL,[reader_name] [nvarchar](50) NULL,[reader_state] [int] NULL,PRIMARY KEY CLUSTERED ([id] ASC) ON [PRIMARY] ) ON [PRIMARY];CREATE TABLE [acc_auxiliary]([id] [bigint] IDENTITY(1,1) NOT NULL,[device_id] [int] NULL,[aux_no] [int] NULL,[printer_number] [nvarchar](50) NULL,[aux_name] [nvarchar](50) NULL,[aux_state] [int] NULL,PRIMARY KEY CLUSTERED ([id] ASC) ON [PRIMARY] ) ON [PRIMARY];");
							}
						}
						catch (Exception)
						{
						}
						num = 8;
					}
					if (num < 9)
					{
						try
						{
							if (AppSite.Instance.DataType == DataType.Access)
							{
								DbHelperOleDb.ExecuteSql("alter table acc_door add ManualCtlMode integer;");
							}
							else if (AppSite.Instance.DataType == DataType.SqlServer)
							{
								DbHelperSQL.ExecuteSql("alter table acc_door add ManualCtlMode int;");
							}
						}
						catch (Exception ex3)
						{
							SysLogServer.WriteLog("Error On Update DataBase to JapanAF：" + ex3.Message, true);
						}
						num = 9;
					}
					if (num < 10)
					{
						List<string> list2 = new List<string>();
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						if (AppSite.Instance.DataType == DataType.Access)
						{
							dictionary.Add("DevSDKType", "integer");
							dictionary.Add("UTableDesc", "LongBinary");
							dictionary.Add("IsTFTMachine", "YESNO");
							dictionary.Add("PinWidth", "integer");
							dictionary.Add("UserExtFmt", "integer");
							dictionary.Add("FP1_NThreshold", "integer");
							dictionary.Add("FP1_1Threshold", "integer");
							dictionary.Add("Face1_NThreshold", "integer");
							dictionary.Add("Face1_1Threshold", "integer");
							dictionary.Add("Only1_1Mode", "integer");
							dictionary.Add("OnlyCheckCard", "integer");
							dictionary.Add("MifireMustRegistered", "integer");
							dictionary.Add("RFCardOn", "integer");
							dictionary.Add("Mifire", "integer");
							dictionary.Add("MifireId", "integer");
							dictionary.Add("NetOn", "integer");
							dictionary.Add("RS232On", "integer");
							dictionary.Add("RS485On", "integer");
							dictionary.Add("FreeType", "integer");
							dictionary.Add("FreeTime", "integer");
							dictionary.Add("NoDisplayFun", "integer");
							dictionary.Add("VoiceTipsOn", "integer");
							dictionary.Add("TOMenu", "integer");
							dictionary.Add("StdVolume", "integer");
							dictionary.Add("VRYVH", "integer");
							dictionary.Add("KeyPadBeep", "integer");
							dictionary.Add("BatchUpdate", "integer");
							dictionary.Add("CardFun", "integer");
							dictionary.Add("FaceFunOn", "integer");
							dictionary.Add("FaceCount", "integer");
							list2.AddRange(DatabaseUpdate.GenerateAddColumnSql("Machines", dictionary));
							list2.Add("CREATE TABLE STD_WiegandFmt([id] autoincrement primary key,[DoorId] integer NULL,[OutWiegandFmt] varchar(100) NULL,[OutFailureId] integer NULL,[OutAreaCode] integer NULL,[OutPulseWidth] integer NULL,[OutPulseInterval] integer NULL,[OutContent] integer NULL,[InWiegandFmt] varchar(100) NULL,[InBitCount] integer NULL,[InPulseWidth] integer NULL,[InPulseInterval] integer NULL,[InContent] integer NULL);");
							list2.Add("alter table acc_door add ErrTimes integer,SensorAlarmTime integer");
							list2.Add("alter table FaceTemp add FaceType byte");
							for (int num6 = 0; num6 < list2.Count; num6++)
							{
								try
								{
									DbHelperOleDb.ExecuteSql(list2[num6]);
								}
								catch (Exception ex4)
								{
									SysLogServer.WriteLog("Error On Update DataBase to StandAloneSDK：" + ex4.Message, true);
								}
							}
						}
						else if (AppSite.Instance.DataType == DataType.SqlServer)
						{
							dictionary.Add("DevSDKType", "int");
							dictionary.Add("UTableDesc", "image");
							dictionary.Add("IsTFTMachine", "bit");
							dictionary.Add("PinWidth", "int");
							dictionary.Add("UserExtFmt", "int");
							dictionary.Add("FP1_NThreshold", "int");
							dictionary.Add("FP1_1Threshold", "int");
							dictionary.Add("Face1_NThreshold", "int");
							dictionary.Add("Face1_1Threshold", "int");
							dictionary.Add("Only1_1Mode", "int");
							dictionary.Add("OnlyCheckCard", "int");
							dictionary.Add("MifireMustRegistered", "int");
							dictionary.Add("RFCardOn", "int");
							dictionary.Add("Mifire", "int");
							dictionary.Add("MifireId", "int");
							dictionary.Add("NetOn", "int");
							dictionary.Add("RS232On", "int");
							dictionary.Add("RS485On", "int");
							dictionary.Add("FreeType", "int");
							dictionary.Add("FreeTime", "int");
							dictionary.Add("NoDisplayFun", "int");
							dictionary.Add("VoiceTipsOn", "int");
							dictionary.Add("TOMenu", "int");
							dictionary.Add("StdVolume", "int");
							dictionary.Add("VRYVH", "int");
							dictionary.Add("KeyPadBeep", "int");
							dictionary.Add("BatchUpdate", "int");
							dictionary.Add("CardFun", "int");
							dictionary.Add("FaceFunOn", "int");
							dictionary.Add("FaceCount", "int");
							list2.AddRange(DatabaseUpdate.GenerateAddColumnSql("Machines", dictionary));
							list2.Add("CREATE TABLE STD_WiegandFmt([id] [int] IDENTITY(1,1) NOT NULL,[DoorId] [int] NULL,[OutWiegandFmt] [nvarchar](100) NULL,[OutFailureId] [int] NULL,[OutAreaCode] [int] NULL,[OutPulseWidth] [int] NULL,[OutPulseInterval] [int] NULL,[OutContent] [int] NULL,[InWiegandFmt] [nvarchar](100) NULL,[InBitCount] [int] NULL,[InPulseWidth] [int] NULL,[InPulseInterval] [int] NULL,[InContent] [int] NULL, CONSTRAINT [PK_STD_WiegandFmt] PRIMARY KEY CLUSTERED ([id] ASC) ON [PRIMARY]) ON [PRIMARY];");
							list2.Add("alter table acc_door add ErrTimes int,SensorAlarmTime int");
							list2.Add("alter table FaceTemp add FaceType tinyint");
							for (int num7 = 0; num7 < list2.Count; num7++)
							{
								try
								{
									DbHelperSQL.ExecuteSql(list2[num7]);
								}
								catch (Exception ex5)
								{
									SysLogServer.WriteLog("Error On Update DataBase to StandAloneSDK：" + ex5.Message, true);
								}
							}
						}
						num = 10;
					}
					if (num < 11)
					{
						List<string> list2 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
						{
							List<string> columnsInTable = DbHelperOleDb.GetColumnsInTable("Machines");
							if (!columnsInTable.Contains("TimeAPBFunOn".ToUpper()))
							{
								list2.Add("alter table Machines add TimeAPBFunOn integer;");
							}
							columnsInTable = DbHelperOleDb.GetColumnsInTable("acc_door");
							if (!columnsInTable.Contains("InTimeAPB".ToUpper()))
							{
								list2.Add("alter table acc_door add InTimeAPB integer;");
							}
							break;
						}
						case DataType.SqlServer:
							list2.Add("if COL_LENGTH('Machines','TimeAPBFunOn') is null alter table Machines add TimeAPBFunOn int;");
							list2.Add("if COL_LENGTH('acc_door','InTimeAPB') is null alter table acc_door add InTimeAPB int;");
							break;
						}
						for (int num8 = 0; num8 < list2.Count; num8++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[num8]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[num8]);
									break;
								}
							}
							catch (Exception ex6)
							{
								SysLogServer.WriteLog("Error On Update DataBase to InTimeAPB version：" + ex6.Message, true);
							}
						}
						num = 11;
					}
					if (num < 12)
					{
						List<string> list2 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
						{
							List<string> columnsInTable2 = DbHelperOleDb.GetColumnsInTable("acc_timeseg");
							if (!columnsInTable2.Contains("TimeZone1Id".ToUpper()))
							{
								list2.Add("alter table acc_timeseg add TimeZone1Id integer;");
							}
							if (!columnsInTable2.Contains("TimeZone2Id".ToUpper()))
							{
								list2.Add("alter table acc_timeseg add TimeZone2Id integer;");
							}
							if (!columnsInTable2.Contains("TimeZone3Id".ToUpper()))
							{
								list2.Add("alter table acc_timeseg add TimeZone3Id integer;");
							}
							if (!columnsInTable2.Contains("TimeZoneHolidayId".ToUpper()))
							{
								list2.Add("alter table acc_timeseg add TimeZoneHolidayId integer;");
							}
							columnsInTable2 = DbHelperOleDb.GetColumnsInTable("acc_holidays");
							if (!columnsInTable2.Contains("HolidayNo".ToUpper()))
							{
								list2.Add("alter table acc_holidays add HolidayNo integer;");
							}
							if (!columnsInTable2.Contains("HolidayTZ".ToUpper()))
							{
								list2.Add("alter table acc_holidays add HolidayTZ integer;");
							}
							columnsInTable2 = DbHelperOleDb.GetColumnsInTable("acc_morecardempgroup");
							if (!columnsInTable2.Contains("StdGroupNo".ToUpper()))
							{
								list2.Add("alter table acc_morecardempgroup add StdGroupNo integer;");
							}
							if (!columnsInTable2.Contains("StdGroupTz".ToUpper()))
							{
								list2.Add("alter table acc_morecardempgroup add StdGroupTz integer;");
							}
							if (!columnsInTable2.Contains("StdGroupVT".ToUpper()))
							{
								list2.Add("alter table acc_morecardempgroup add StdGroupVT integer;");
							}
							if (!columnsInTable2.Contains("StdValidOnHoliday".ToUpper()))
							{
								list2.Add("alter table acc_morecardempgroup add StdValidOnHoliday YESNO;");
							}
							columnsInTable2 = DbHelperOleDb.GetColumnsInTable("acc_morecardset");
							if (!columnsInTable2.Contains("CombNo".ToUpper()))
							{
								list2.Add("alter table acc_morecardset add CombNo integer;");
							}
							break;
						}
						case DataType.SqlServer:
							list2.Add("if COL_LENGTH('acc_timeseg','TimeZone1Id') is null alter table acc_timeseg add TimeZone1Id int;");
							list2.Add("if COL_LENGTH('acc_timeseg','TimeZone2Id') is null alter table acc_timeseg add TimeZone2Id int;");
							list2.Add("if COL_LENGTH('acc_timeseg','TimeZone3Id') is null alter table acc_timeseg add TimeZone3Id int;");
							list2.Add("if COL_LENGTH('acc_timeseg','TimeZoneHolidayId') is null alter table acc_timeseg add TimeZoneHolidayId int;");
							list2.Add("if COL_LENGTH('acc_holidays','HolidayNo') is null alter table acc_holidays add HolidayNo int;");
							list2.Add("if COL_LENGTH('acc_holidays','HolidayTZ') is null alter table acc_holidays add HolidayTZ int;");
							list2.Add("if COL_LENGTH('acc_morecardempgroup','StdGroupNo') is null alter table acc_morecardempgroup add StdGroupNo int;");
							list2.Add("if COL_LENGTH('acc_morecardempgroup','StdGroupTz') is null alter table acc_morecardempgroup add StdGroupTz int;");
							list2.Add("if COL_LENGTH('acc_morecardempgroup','StdGroupVT') is null alter table acc_morecardempgroup add StdGroupVT int;");
							list2.Add("if COL_LENGTH('acc_morecardempgroup','StdValidOnHoliday') is null alter table acc_morecardempgroup add StdValidOnHoliday bit;");
							list2.Add("if COL_LENGTH('acc_morecardset','CombNo') is null alter table acc_morecardset add CombNo int;");
							break;
						}
						for (int num9 = 0; num9 < list2.Count; num9++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[num9]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[num9]);
									break;
								}
							}
							catch (Exception ex7)
							{
								SysLogServer.WriteLog("Error On Update DataBase to StdSYNC version：" + ex7.Message, true);
							}
						}
						num = 12;
					}
					if (num < 13)
					{
						bool flag = false;
						AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
						List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
						if (modelList != null && modelList.Count > 0)
						{
							for (int num10 = 0; num10 < modelList.Count; num10++)
							{
								AccTimeseg accTimeseg = modelList[num10];
								if (accTimeseg.TimeZone1Id == 1 || accTimeseg.TimeZone2Id == 1 || accTimeseg.TimeZone3Id == 1 || accTimeseg.TimeZoneHolidayId == 1)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							AccTimeseg accTimeseg = DeviceHelper.TimeSeg;
							if (accTimeseg.TimeZone1Id <= 0)
							{
								accTimeseg.TimeZone1Id = 1;
								accTimesegBll.Update(accTimeseg);
							}
						}
						num = 13;
					}
					if (num < 14)
					{
						List<string> list2 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
							list2.Add("alter table machines alter column FirmwareVersion varchar(50)");
							break;
						case DataType.SqlServer:
							list2.Add("alter table machines alter column FirmwareVersion nvarchar(50)");
							break;
						}
						for (int num11 = 0; num11 < list2.Count; num11++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[num11]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[num11]);
									break;
								}
							}
							catch (Exception ex8)
							{
								SysLogServer.WriteLog("Error On Update DataBase to LongFirmwareVer version：" + ex8.Message, true);
							}
						}
						num = 14;
					}
					if (num < 16)
					{
						List<string> list2 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
							list2.Add("CREATE TABLE CustomReport ([id] autoincrement primary key, [ReportName] varchar(50), [Memo] varchar(50));");
							list2.Add("CREATE TABLE ReportField (CRId integer, [TableName] varchar(50), [FieldName] varchar(50), ShowIndex Integer);");
							list2.Add("alter table ReportField add constraint PK_ReportField primary key (CRId,[TableName],[FieldName]);");
							break;
						case DataType.SqlServer:
							list2.Add("CREATE TABLE CustomReport ([id] [int] IDENTITY(1,1) NOT NULL, ReportName nvarchar(50), Memo nvarchar(50));");
							list2.Add("CREATE TABLE ReportField (CRId integer not null, [TableName] varchar(50) not null, [FieldName] varchar(50) not null, ShowIndex Integer not null);");
							list2.Add("alter table ReportField add constraint PK_ReportField primary key (CRId,[TableName],[FieldName]);");
							break;
						}
						for (int num12 = 0; num12 < list2.Count; num12++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[num12]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[num12]);
									break;
								}
							}
							catch (Exception ex9)
							{
								SysLogServer.WriteLog("Error On Update DataBase to CustomReport version：" + ex9.Message, true);
							}
						}
						num = 16;
					}
					if (num < 17)
					{
						List<string> list2 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
							list2.Add("alter table machines add column FingerFunOn varchar(5)");
							list2.Add("alter table machines add column CompatOldFirmware varchar(5)");
							break;
						case DataType.SqlServer:
							list2.Add("alter table machines add FingerFunOn nvarchar(5)");
							list2.Add("alter table machines add CompatOldFirmware nvarchar(5)");
							break;
						}
						for (int num13 = 0; num13 < list2.Count; num13++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[num13]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[num13]);
									break;
								}
							}
							catch (Exception ex10)
							{
								SysLogServer.WriteLog("Error On Update DataBase to CompactFv300 version：" + ex10.Message, true);
							}
						}
						num = 17;
					}
					if (num < 18)
					{
						List<string> list2 = new List<string>();
						switch (AppSite.Instance.DataType)
						{
						case DataType.Access:
							list2.Add("alter table FingerVein add column Fv_ID_Index integer");
							break;
						case DataType.SqlServer:
							list2.Add("alter table FingerVein add Fv_ID_Index int");
							break;
						}
						for (int num14 = 0; num14 < list2.Count; num14++)
						{
							try
							{
								switch (AppSite.Instance.DataType)
								{
								case DataType.Access:
									DbHelperOleDb.ExecuteSql(list2[num14]);
									break;
								case DataType.SqlServer:
									DbHelperSQL.ExecuteSql(list2[num14]);
									break;
								}
							}
							catch (Exception ex11)
							{
								SysLogServer.WriteLog("Error On Update DataBase to CompactFv300_2 version：" + ex11.Message, true);
							}
						}
						num = 18;
					}
					if (num < 19)
					{
						WaitForm instance2 = WaitForm.Instance;
						try
						{
							string[][] array = new string[5][]
							{
								new string[2]
								{
									"common",
									"alter table USERINFO add isVisitor char(1) default 'N'"
								},
								new string[2]
								{
									"common",
									"alter table acc_door add isCardBox char(1) default 'N'"
								},
								new string[2]
								{
									"common",
									"alter table acc_levelset add isVisitLevel char(1) default 'N'"
								},
								new string[2]
								{
									"access",
									"CREATE  TABLE vis_visit_info (\r\n    id AUTOINCREMENT PRIMARY KEY,\r\n    userinfo_id INTEGER NOT NULL ,\r\n    pin VARCHAR(20) NOT NULL ,\r\n    card VARCHAR(20) NOT NULL ,\r\n    start_date DATETIME NOT NULL ,\r\n    end_date DATETIME NOT NULL ,\r\n    create_date DATETIME NOT NULL ,\r\n    active CHAR(1) NOT NULL DEFAULT 'N')"
								},
								new string[2]
								{
									"sql",
									"CREATE  TABLE vis_visit_info (\r\n    id INT IDENTITY PRIMARY KEY,\r\n    userinfo_id INT NOT NULL ,\r\n    pin VARCHAR(20) NOT NULL ,\r\n    card VARCHAR(20) NOT NULL ,\r\n    start_date DATETIME NOT NULL ,\r\n    end_date DATETIME NOT NULL ,\r\n    create_date DATETIME NOT NULL ,\r\n    active CHAR(1) NOT NULL DEFAULT 'N')"
								}
							};
							for (int num15 = 0; num15 < array.Length; num15++)
							{
								try
								{
									if (AppSite.Instance.DataType == DataType.Access)
									{
										if (array[num15][0].Equals("common") || array[num15][0].Equals("access"))
										{
											DbHelperOleDb.ExecuteSql(array[num15][1]);
										}
									}
									else if (AppSite.Instance.DataType == DataType.SqlServer && (array[num15][0].Equals("common") || array[num15][0].Equals("sql")))
									{
										DbHelperSQL.ExecuteSql(array[num15][1]);
									}
								}
								catch (Exception ex12)
								{
									instance2.ShowEx(ex12.Message);
								}
							}
							num = 19;
						}
						catch
						{
						}
					}
					if (num < 20)
					{
						WaitForm instance3 = WaitForm.Instance;
						try
						{
							string[] array2 = new string[7]
							{
								"alter table USERINFO add elevatorProfile varchar(20)",
								"alter table USERINFO add elevatorDefaultFloor varchar(20)",
								"alter table USERINFO add elevatorSpecialNeeds char(1) default 'N'",
								"alter table Machines add elevatorTerminalId int",
								"alter table Machines add elevatorSourceFloor varchar(20)",
								"alter table Machines add elevatorServerPort varchar(5)",
								"alter table Machines add elevatorServerIp varchar(15)"
							};
							for (int num16 = 0; num16 < array2.Length; num16++)
							{
								try
								{
									if (AppSite.Instance.DataType == DataType.Access)
									{
										DbHelperOleDb.ExecuteSql(array2[num16]);
									}
									else if (AppSite.Instance.DataType == DataType.SqlServer)
									{
										DbHelperSQL.ExecuteSql(array2[num16]);
									}
								}
								catch (Exception ex13)
								{
									instance3.ShowEx(ex13.Message);
								}
							}
							num = 20;
						}
						catch
						{
						}
					}
					if (num < 21)
					{
						WaitForm instance4 = WaitForm.Instance;
						try
						{
							string[] array3 = new string[2]
							{
								"alter table acc_monitor_log add user_name varchar(100)",
								"alter table acc_monitor_log add user_lastname varchar(100)"
							};
							for (int num17 = 0; num17 < array3.Length; num17++)
							{
								try
								{
									if (AppSite.Instance.DataType == DataType.Access)
									{
										DbHelperOleDb.ExecuteSql(array3[num17]);
									}
									else if (AppSite.Instance.DataType == DataType.SqlServer)
									{
										DbHelperSQL.ExecuteSql(array3[num17]);
									}
								}
								catch (Exception ex14)
								{
									instance4.ShowEx(ex14.Message);
								}
							}
							num = 21;
						}
						catch
						{
						}
					}
					DatabaseUpdate.attParam.PARAVALUE = num.ToString();
					DatabaseUpdate.attParamBll.Update(DatabaseUpdate.attParam);
				}
			}
			catch (Exception ex15)
			{
				SysLogServer.WriteLog("Error On Update DataBase：" + ex15.Message, true);
			}
		}

		private static int GetLastVersion()
		{
			try
			{
				DatabaseUpdate.attParam = DatabaseUpdate.attParamBll.GetModel("DBVersionAccess");
				if (DatabaseUpdate.attParam != null)
				{
					return int.Parse(DatabaseUpdate.attParam.PARAVALUE);
				}
				AttParam attParam = new AttParam();
				attParam.PARANAME = "DBVersionAccess";
				attParam.PARATYPE = null;
				attParam.PARAVALUE = string.Empty;
				DatabaseUpdate.attParamBll.Add(attParam);
				DatabaseUpdate.attParam = attParam;
				return 0;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public static int IsECardTongSign()
		{
			try
			{
				AttParam attParam = null;
				attParam = DatabaseUpdate.attParamBll.GetModel("IsECardTong");
				if (attParam != null)
				{
					return int.Parse(attParam.PARAVALUE);
				}
				return 0;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		public static List<string> GenerateAddColumnSql(string TableName, Dictionary<string, string> dicColumnName_ColumnType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			DataType dataType = AppSite.Instance.DataType;
			if (dataType == DataType.SqlServer)
			{
				string format = string.Format("if COL_LENGTH('{0}','{{0}}') is null alter table {0} add {{0}} {{1}};", TableName);
				foreach (KeyValuePair<string, string> item in dicColumnName_ColumnType)
				{
					list.Add(string.Format(format, item.Key, item.Value));
				}
			}
			else
			{
				string format = $"Alter Table {TableName} Add {{0}} {{1}};";
				List<string> columnsInTable = DbHelperOleDb.GetColumnsInTable(TableName);
				foreach (KeyValuePair<string, string> item2 in dicColumnName_ColumnType)
				{
					if (!columnsInTable.Contains(item2.Key.ToUpper()))
					{
						list.Add(string.Format(format, item2.Key, item2.Value));
					}
				}
			}
			return list;
		}
	}
}
