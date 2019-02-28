CREATE TABLE [dbo].[acc_antiback](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [smallint] NOT NULL CONSTRAINT [default_value_acc_antiback_status]  DEFAULT ('0'),
	[device_id] [int] NULL,
	[one_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_one_mode]  DEFAULT ('False'),
	[two_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_two_mode]  DEFAULT ('False'),
	[three_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_three_mode]  DEFAULT ('False'),
	[four_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_four_mode]  DEFAULT ('False'),
	[five_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_five_mode]  DEFAULT ('False'),
	[six_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_six_mode]  DEFAULT ('False'),
	[seven_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_seven_mode]  DEFAULT ('False'),
	[eight_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_eight_mode]  DEFAULT ('False'),
	[nine_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_antiback_nine_mode]  DEFAULT ('False'),
        [AntibackType] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_door](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_door_status]  DEFAULT ('0'),
	[device_id] [int] NULL,
	[door_no] [smallint] NULL,
	[door_name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_door_door_name]  DEFAULT (''),
	[lock_delay] [int] NULL CONSTRAINT [default_value_acc_door_lock_delay]  DEFAULT ('5'),
	[back_lock] [bit] NOT NULL CONSTRAINT [default_value_acc_door_back_lock]  DEFAULT ('True'),
	[sensor_delay] [int] NULL CONSTRAINT [default_value_acc_door_sensor_delay]  DEFAULT ('15'),
	[opendoor_type] [int] NULL CONSTRAINT [default_value_acc_door_opendoor_type]  DEFAULT ('6'),
	[inout_state] [int] NULL CONSTRAINT [default_value_acc_door_inout_state]  DEFAULT ('0'),
	[lock_active_id] [int] NULL CONSTRAINT [default_value_acc_door_lock_active_id]  DEFAULT ('1'),
	[long_open_id] [int] NULL,
	[wiegand_fmt_id] [int] NULL CONSTRAINT [default_value_acc_door_wiegand_fmt_id]  DEFAULT ('1'),
	[card_intervaltime] [int] NULL CONSTRAINT [default_value_acc_door_card_intervaltime]  DEFAULT ('2'),
	[reader_type] [int] NULL CONSTRAINT [default_value_acc_door_reader_type]  DEFAULT ('0'),
	[is_att] [bit] NOT NULL CONSTRAINT [default_value_acc_door_is_att]  DEFAULT ('False'),
	[force_pwd] [nvarchar](100) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_door_force_pwd]  DEFAULT (''),
	[supper_pwd] [nvarchar](100) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_door_supper_pwd]  DEFAULT (''),
	[door_sensor_status] [int] NULL CONSTRAINT [default_value_acc_door_door_sensor_status]  DEFAULT ('0'),
	[map_id] [int] NULL,
	[duration_apb] [int] NULL,
        [reader_io_state] [int] NULL DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;
ALTER TABLE [dbo].[acc_door]  WITH CHECK ADD CHECK  (([card_intervaltime]>=(0)));
ALTER TABLE [dbo].[acc_door]  WITH CHECK ADD CHECK  (([door_no]>=(0)));
ALTER TABLE [dbo].[acc_door]  WITH CHECK ADD CHECK  (([lock_delay]>=(0)));
ALTER TABLE [dbo].[acc_door]  WITH CHECK ADD CHECK  (([sensor_delay]>=(0)));


CREATE TABLE [dbo].[acc_firstopen](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_firstopen_status]  DEFAULT ('0'),
	[door_id] [int] NULL CONSTRAINT [default_value_acc_firstopen_door_id]  DEFAULT ('1'),
	[timeseg_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_firstopen_emp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[accfirstopen_id] [int] NULL,
	[employee_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[accfirstopen_id] ASC,
	[employee_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_holidays](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_holidays_status]  DEFAULT ('0'),
	[holiday_name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_holidays_holiday_name]  DEFAULT (''),
	[holiday_type] [int] NULL CONSTRAINT [default_value_acc_holidays_holiday_type]  DEFAULT ('1'),
	[start_date] [datetime] NOT NULL,
	[end_date] [datetime] NOT NULL,
	[loop_by_year] [int] NULL CONSTRAINT [default_value_acc_holidays_loop_by_year]  DEFAULT ('2'),
	[memo] [nvarchar](70) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_holidays_memo]  DEFAULT (''),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[holiday_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_interlock](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [int] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_interlock_status]  DEFAULT ('0'),
	[device_id] [int] NULL,
	[one_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_interlock_one_mode]  DEFAULT ('False'),
	[two_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_interlock_two_mode]  DEFAULT ('False'),
	[three_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_interlock_three_mode]  DEFAULT ('False'),
	[four_mode] [bit] NOT NULL CONSTRAINT [default_value_acc_interlock_four_mode]  DEFAULT ('False'),
	[InterlockType] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[device_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_levelset](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [smallint] NOT NULL CONSTRAINT [default_value_acc_levelset_status]  DEFAULT ('0'),
	[level_name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_levelset_level_name]  DEFAULT (''),
	[level_timeseg_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[level_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_levelset_door_group](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[acclevelset_id] [int] NOT NULL,
	[accdoor_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[acclevelset_id] ASC,
	[accdoor_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_levelset_emp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[acclevelset_id] [int] NOT NULL,
	[employee_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[acclevelset_id] ASC,
	[employee_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_linkageio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[status] [int] NULL CONSTRAINT [default_value_acc_linkageio_status]  DEFAULT ('0'),
	[linkage_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[device_id] [int] NULL,
	[trigger_opt] [int] NULL CONSTRAINT [default_value_acc_linkageio_trigger_opt]  DEFAULT ('0'),
	[in_address_hide] [int] NULL,
	[in_address] [int] NULL CONSTRAINT [default_value_acc_linkageio_in_address]  DEFAULT ('0'),
	[out_type_hide] [int] NULL,
	[out_address_hide] [int] NULL,
	[out_address] [int] NULL,
	[action_type] [int] NULL,
	[delay_time] [int] NULL CONSTRAINT [default_value_acc_linkageio_action_type]  DEFAULT ('0'),
	[video_linkageio_id] [int] NULL CONSTRAINT [default_value_acc_linkageio_delay_time]  DEFAULT ('20'),
	[lchannel_num] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[linkage_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_map](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_map_status]  DEFAULT ('0'),
	[map_name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_map_map_name]  DEFAULT (''),
	[map_path] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL,
	[area_id] [int] NULL,
	[width] [int] NULL CONSTRAINT [default_value_acc_map_width]  DEFAULT ('0'),
	[height] [int] NULL CONSTRAINT [default_value_acc_map_height]  DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[map_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_mapdoorpos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_mapdoorpos_status]  DEFAULT ('0'),
	[map_door_id] [int] NULL,
	[map_id] [int] NULL,
	[width] [int] NULL CONSTRAINT [default_value_acc_mapdoorpos_width]  DEFAULT ('40'),
	[left] [int] NULL,
	[top] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_monitor_log](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_monitor_log_status]  DEFAULT ('0'),
	[log_tag] [int] NULL,
	[time] [datetime] NULL,
	[pin] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[card_no] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[device_id] [int] NULL,
	[device_sn] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[device_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[verified] [int] NULL CONSTRAINT [default_value_acc_monitor_log_verified]  DEFAULT ('200'),
	[state] [int] NULL,
	[event_type] [int] NULL,
        [description] [nvarchar](200) NULL,
        [event_point_type] [int] NULL DEFAULT ('-1'),
        [event_point_id] [int] NULL DEFAULT ('-1'),
        [event_point_name] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [time_pin] UNIQUE NONCLUSTERED 
(
	[time] ASC,
	[pin] ASC,
	[card_no] ASC,
	[device_id] ASC,
	[verified] ASC,
	[state] ASC,
	[event_type] ASC,
        [description] ASC,
        [event_point_type] ASC,
        [event_point_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[time] ASC,
	[pin] ASC,
	[card_no] ASC,
	[device_id] ASC,
	[verified] ASC,
	[state] ASC,
	[event_type] ASC,
        [description] ASC,
        [event_point_type] ASC,
        [event_point_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_morecardempgroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_morecardempgroup_status]  DEFAULT ('0'),
	[group_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[memo] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[group_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_morecardgroup](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_morecardgroup_status]  DEFAULT ('0'),
	[comb_id] [int] NULL,
	[group_id] [int] NULL,
	[opener_number] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_morecardset](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_morecardset_status]  DEFAULT ('0'),
	[door_id] [int] NULL,
	[comb_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[comb_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_timeseg](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_acc_timeseg_status]  DEFAULT ('0'),
	[timeseg_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[memo] [nvarchar](70) COLLATE Chinese_PRC_CI_AS NULL CONSTRAINT [default_value_acc_timeseg_memo]  DEFAULT (''),
	[sunday_start1] [datetime] NOT NULL CONSTRAINT [default_value_acc_timeseg_sunday_start1]  DEFAULT ('00:00'),
	[sunday_end1] [datetime] NULL DEFAULT ('00:00'),
	[sunday_start2] [datetime] NULL DEFAULT ('00:00'),
	[sunday_end2] [datetime] NULL DEFAULT ('00:00'),
	[sunday_start3] [datetime] NULL DEFAULT ('00:00'),
	[sunday_end3] [datetime] NULL DEFAULT ('00:00'),
	[monday_start1] [datetime] NULL DEFAULT ('00:00'),
	[monday_end1] [datetime] NULL DEFAULT ('00:00'),
	[monday_start2] [datetime] NULL DEFAULT ('00:00'),
	[monday_end2] [datetime] NULL DEFAULT ('00:00'),
	[monday_start3] [datetime] NULL DEFAULT ('00:00'),
	[monday_end3] [datetime] NULL DEFAULT ('00:00'),
	[tuesday_start1] [datetime] NULL DEFAULT ('00:00'),
	[tuesday_end1] [datetime] NULL DEFAULT ('00:00'),
	[tuesday_start2] [datetime] NULL DEFAULT ('00:00'),
	[tuesday_end2] [datetime] NULL DEFAULT ('00:00'),
	[tuesday_start3] [datetime] NULL DEFAULT ('00:00'),
	[tuesday_end3] [datetime] NULL DEFAULT ('00:00'),
	[wednesday_start1] [datetime] NULL DEFAULT ('00:00'),
	[wednesday_end1] [datetime] NULL DEFAULT ('00:00'),
	[wednesday_start2] [datetime] NULL DEFAULT ('00:00'),
	[wednesday_end2] [datetime] NULL DEFAULT ('00:00'),
	[wednesday_start3] [datetime] NULL DEFAULT ('00:00'),
	[wednesday_end3] [datetime] NULL DEFAULT ('00:00'),
	[thursday_start1] [datetime] NULL DEFAULT ('00:00'),
	[thursday_end1] [datetime] NULL DEFAULT ('00:00'),
	[thursday_start2] [datetime] NULL DEFAULT ('00:00'),
	[thursday_end2] [datetime] NULL DEFAULT ('00:00'),
	[thursday_start3] [datetime] NULL DEFAULT ('00:00'),
	[thursday_end3] [datetime] NULL DEFAULT ('00:00'),
	[friday_start1] [datetime] NULL DEFAULT ('00:00'),
	[friday_end1] [datetime] NULL DEFAULT ('00:00'),
	[friday_start2] [datetime] NULL DEFAULT ('00:00'),
	[friday_end2] [datetime] NULL DEFAULT ('00:00'),
	[friday_start3] [datetime] NULL DEFAULT ('00:00'),
	[friday_end3] [datetime] NULL DEFAULT ('00:00'),
	[saturday_start1] [datetime] NULL DEFAULT ('00:00'),
	[saturday_end1] [datetime] NULL DEFAULT ('00:00'),
	[saturday_start2] [datetime] NULL DEFAULT ('00:00'),
	[saturday_end2] [datetime] NULL DEFAULT ('00:00'),
	[saturday_start3] [datetime] NULL DEFAULT ('00:00'),
	[saturday_end3] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype1_start1] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype1_end1] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype1_start2] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype1_end2] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype1_start3] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype1_end3] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype2_start1] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype2_end1] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype2_start2] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype2_end2] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype2_start3] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype2_end3] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype3_start1] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype3_end1] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype3_start2] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype3_end2] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype3_start3] [datetime] NULL DEFAULT ('00:00'),
	[holidaytype3_end3] [datetime] NULL DEFAULT ('00:00'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[timeseg_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acc_wiegandfmt](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[wiegand_name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [default_value_acc_wiegandfmt_wiegand_name]  DEFAULT (''),
	[wiegand_count] [int] NULL,
	[odd_start] [int] NULL,
	[odd_count] [int] NULL,
	[even_start] [int] NULL,
	[even_count] [int] NULL,
	[cid_start] [int] NULL,
	[cid_count] [int] NULL,
	[comp_start] [int] NULL,
	[comp_count] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[ACGroup](
	[GroupID] [smallint] NOT NULL,
	[Name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[TimeZone1] [smallint] NULL,
	[TimeZone2] [smallint] NULL,
	[TimeZone3] [smallint] NULL,
	[holidayvaild] [bit] NOT NULL,
	[verifystyle] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[acholiday](
	[primaryid] [int] IDENTITY(1,1) NOT NULL,
	[holidayid] [int] NULL,
	[begindate] [datetime] NULL,
	[enddate] [datetime] NULL,
	[timezone] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[primaryid] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[ACTimeZones](
	[TimeZoneID] [smallint] NOT NULL,
	[Name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[SunStart] [datetime] NULL,
	[SunEnd] [datetime] NULL,
	[MonStart] [datetime] NULL,
	[MonEnd] [datetime] NULL,
	[TuesStart] [datetime] NULL,
	[TuesEnd] [datetime] NULL,
	[WedStart] [datetime] NULL,
	[WedEnd] [datetime] NULL,
	[ThursStart] [datetime] NULL,
	[ThursEnd] [datetime] NULL,
	[FriStart] [datetime] NULL,
	[FriEnd] [datetime] NULL,
	[SatStart] [datetime] NULL,
	[SatEnd] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TimeZoneID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[action_log](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[action_time] [datetime] NULL,
	[user_id] [int] NULL,
	[content_type_id] [int] NULL,
	[object_id] [int] NULL,
	[object_repr] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[action_flag] [int] NULL,
	[change_message] [nvarchar](500) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[ACUnlockComb](
	[UnlockCombID] [smallint] NOT NULL,
	[Name] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[Group01] [smallint] NULL,
	[Group02] [smallint] NULL,
	[Group03] [smallint] NULL,
	[Group04] [smallint] NULL,
	[Group05] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[UnlockCombID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[AlarmLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Operator] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[EnrollNumber] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[LogTime] [datetime] NULL DEFAULT (getdate()),
	[MachineAlias] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[AlarmType] [int] NULL
) ON [PRIMARY]
;

CREATE TABLE [dbo].[areaadmin](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[area_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[att_attreport](
	[ItemName] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ItemType] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Author_id] [int] NULL,
	[ItemValue] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL,
	[Published] [int] NULL CONSTRAINT [default_value_att_attreport_Published]  DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[ItemName] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[att_waitforprocessdata](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_att_waitforprocessdata_status]  DEFAULT ('0'),
	[UserID_id] [int] NULL,
	[starttime] [datetime] NULL,
	[endtime] [datetime] NULL,
	[flag] [int] NOT NULL CONSTRAINT [default_value_att_waitforprocessdata_flag]  DEFAULT ('1'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[attcalclog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DeptID] [int] NULL CONSTRAINT [default_value_attcalclog_DeptID]  DEFAULT ('0'),
	[UserId] [int] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[OperTime] [datetime] NULL,
	[Type] [int] NULL CONSTRAINT [default_value_attcalclog_Type]  DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[attexception](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_attexception_status]  DEFAULT ('0'),
	[UserId] [int] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[ExceptionID] [int] NULL CONSTRAINT [default_value_attexception_ExceptionID]  DEFAULT ('0'),
	[AuditExcID] [int] NULL CONSTRAINT [default_value_attexception_AuditExcID]  DEFAULT ('0'),
	[OldAuditExcID] [int] NULL CONSTRAINT [default_value_attexception_OldAuditExcID]  DEFAULT ('0'),
	[OverlapTime] [int] NULL CONSTRAINT [default_value_attexception_OverlapTime]  DEFAULT ('0'),
	[TimeLong] [int] NULL CONSTRAINT [default_value_attexception_TimeLong]  DEFAULT ('0'),
	[InScopeTime] [int] NULL CONSTRAINT [default_value_attexception_InScopeTime]  DEFAULT ('0'),
	[AttDate] [datetime] NULL,
	[OverlapWorkDayTai] [int] NULL CONSTRAINT [default_value_attexception_OverlapWorkDay]  DEFAULT ('1'),
	[OverlapWorkDay] [int] NULL,
	[schindex] [int] NULL CONSTRAINT [default_value_attexception_schindex]  DEFAULT ('0'),
	[Minsworkday] [int] NULL CONSTRAINT [default_value_attexception_Minsworkday]  DEFAULT ('0'),
	[Minsworkday1] [int] NULL CONSTRAINT [default_value_attexception_Minsworkday1]  DEFAULT ('0'),
	[schid] [int] NULL CONSTRAINT [default_value_attexception_schid]  DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[AttDate] ASC,
	[StartTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[AttParam](
	[PARANAME] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[PARATYPE] [nvarchar](2) COLLATE Chinese_PRC_CI_AS NULL,
	[PARAVALUE] [nvarchar](100) COLLATE Chinese_PRC_CI_AS NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PARANAME] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[AuditedExc](
	[AEID] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CheckTime] [datetime] NOT NULL,
	[NewExcID] [int] NULL,
	[IsLeave] [smallint] NULL,
	[UName] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[UTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AEID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_group](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Permission] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL,
	[Remark] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_group_permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[group_id] [int] NOT NULL,
	[permission_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[group_id] ASC,
	[permission_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_message](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[message] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_permission](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[content_type_id] [int] NULL,
	[codename] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[content_type_id] ASC,
	[codename] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_user](
	[id] [int] IDENTITY(1,1) NOT NULL primary key,
	[username] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[password] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Status] [int] NULL,
	[last_login] [datetime] NULL,
	[RoleID] [int] NULL,
	[Remark] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_user_groups](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[group_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[user_id] ASC,
	[group_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[auth_user_user_permissions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[permission_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[user_id] ASC,
	[permission_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[AUTHDEVICE](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [int] NOT NULL,
	[MachineID] [int] NOT NULL,
 CONSTRAINT [USERCHECKTIME] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC,
	[MachineID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_additiondata](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[create_time] [datetime] NULL,
	[user_id] [int] NULL,
	[content_type_id] [int] NULL,
	[object_id] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[key] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[data] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_appoption](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[optname] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[discribe] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_basecode](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[content] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[content_class] [int] NULL CONSTRAINT [default_value_base_basecode_content_class]  DEFAULT ('0'),
	[display] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[remark] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[is_add] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_datatranslation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[content_type_id] [int] NULL,
	[property] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[language] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[display] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_operatortemplate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[user_id] [int] NULL,
	[template1] [image] NULL,
	[finger_id] [int] NULL,
	[valid] [int] NULL,
	[fpversion] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[bio_type] [int] NULL,
	[utime] [datetime] NULL,
	[bitmap_picture] [image] NULL,
	[bitmap_picture2] [image] NULL,
	[bitmap_picture3] [image] NULL,
	[bitmap_picture4] [image] NULL,
	[use_type] [int] NULL,
	[template2] [image] NULL,
	[template3] [image] NULL,
	[template_flag] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[user_id] ASC,
	[finger_id] ASC,
	[fpversion] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

CREATE TABLE [dbo].[base_option](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[app_label] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[catalog] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[group] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[widget] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[required] [bit] NOT NULL CONSTRAINT [default_value_base_option_required]  DEFAULT ('False'),
	[validator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[verbose_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[help_text] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[visible] [bit] NOT NULL CONSTRAINT [default_value_base_option_visible]  DEFAULT ('False'),
	[default] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[read_only] [bit] NOT NULL CONSTRAINT [default_value_base_option_read_only]  DEFAULT ('False'),
	[for_personal] [bit] NOT NULL CONSTRAINT [default_value_base_option_for_personal]  DEFAULT ('True'),
	[type] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [default_value_base_option_type]  DEFAULT ('CharField'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_personaloption](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[option_id] [int] NULL,
	[value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[option_id] ASC,
	[user_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_strresource](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[app] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[str] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_strtranslation](
	[id] [int] IDENTITY(1,1) NOT NULL primary key,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[str_id] [int] NULL,
	[language] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[display] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL
) ON [PRIMARY]
;

CREATE TABLE [dbo].[base_systemoption](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[option_id] [int] NULL,
	[value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[CHECKEXACT](
	[EXACTID] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [int] NULL DEFAULT ((0)),
	[CHECKTIME] [datetime] NULL DEFAULT ((0)),
	[CHECKTYPE] [varchar](2) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ((0)),
	[ISADD] [smallint] NULL DEFAULT ((0)),
	[YUYIN] [varchar](25) COLLATE Chinese_PRC_CI_AS NULL,
	[ISMODIFY] [smallint] NULL DEFAULT ((0)),
	[ISDELETE] [smallint] NULL DEFAULT ((0)),
	[INCOUNT] [smallint] NULL DEFAULT ((0)),
	[ISCOUNT] [smallint] NULL DEFAULT ((0)),
	[MODIFYBY] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[DATE] [datetime] NULL,
 CONSTRAINT [EXACTID] PRIMARY KEY CLUSTERED 
(
	[EXACTID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[CHECKINOUT](
	[USERID] [int] NOT NULL,
	[CHECKTIME] [datetime] NOT NULL DEFAULT (getdate()),
	[CHECKTYPE] [varchar](1) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ('I'),
	[VERIFYCODE] [int] NULL DEFAULT ((0)),
	[SENSORID] [varchar](5) COLLATE Chinese_PRC_CI_AS NULL,
	[LOGID] [int] IDENTITY(1,1) NOT NULL,
	[MachineId] [int] NULL,
	[UserExtFmt] [int] NULL,
	[WorkCode] [int] NULL,
	[Memoinfo] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[sn] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [USERCHECKTIME1] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC,
	[CHECKTIME] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[dbapp_viewmodel](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[model_id] [int] NULL,
	[name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[info] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[viewtype] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[dbbackuplog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[user_id] [int] NULL,
	[starttime] [datetime] NULL,
	[imflag] [bit] NOT NULL CONSTRAINT [default_value_dbbackuplog_imflag]  DEFAULT ('False'),
	[successflag] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[DEPARTMENTS](
	[DEPTID] [int] IDENTITY(1,1) NOT NULL,
	[DEPTNAME] [nvarchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[SUPDEPTID] [int] NOT NULL DEFAULT ((1)),
	[InheritParentSch] [smallint] NULL DEFAULT ((1)),
	[InheritDeptSch] [smallint] NULL DEFAULT ((1)),
	[InheritDeptSchClass] [smallint] NULL DEFAULT ((1)),
	[AutoSchPlan] [smallint] NULL DEFAULT ((1)),
	[InLate] [smallint] NULL DEFAULT ((1)),
	[OutEarly] [smallint] NULL DEFAULT ((1)),
	[InheritDeptRule] [smallint] NULL DEFAULT ((1)),
	[MinAutoSchInterval] [int] NULL DEFAULT ((24)),
	[RegisterOT] [smallint] NULL DEFAULT ((1)),
	[DefaultSchId] [int] NOT NULL DEFAULT ((1)),
	[ATT] [smallint] NULL DEFAULT ((1)),
	[Holiday] [smallint] NULL DEFAULT ((1)),
	[OverTime] [smallint] NULL DEFAULT ((1)),
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[code] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[type] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[invalidate] [datetime] NULL,
 CONSTRAINT [DEPTID] PRIMARY KEY CLUSTERED 
(
	[DEPTID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[deptadmin](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[dept_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[DeptUsedSchs](
	[DeptId] [int] NOT NULL,
	[SchId] [int] NOT NULL,
 CONSTRAINT [DeptUsedSchs1] PRIMARY KEY CLUSTERED 
(
	[DeptId] ASC,
	[SchId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[devcmds](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_devcmds_status]  DEFAULT ('0'),
	[SN_id] [int] NULL,
	[CmdOperate_id] [int] NULL,
	[CmdContent] [text] COLLATE Chinese_PRC_CI_AS NOT NULL,
	[CmdCommitTime] [datetime] NOT NULL CONSTRAINT [default_value_devcmds_CmdCommitTime]  DEFAULT ('2011-07-15 16:06:23.608000'),
	[CmdTransTime] [datetime] NULL,
	[CmdOverTime] [datetime] NULL,
	[CmdReturn] [int] NULL,
	[CmdReturnContent] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[CmdImmediately] [bit] NOT NULL CONSTRAINT [default_value_devcmds_CmdImmediately]  DEFAULT ('False'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[devcmds_bak](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NOT NULL CONSTRAINT [default_value_devcmds_bak_status]  DEFAULT ('0'),
	[SN_id] [int] NULL,
	[CmdOperate_id] [int] NULL,
	[CmdContent] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[CmdCommitTime] [datetime] NOT NULL CONSTRAINT [default_value_devcmds_bak_CmdCommitTime]  DEFAULT ('2011-07-15 16:06:23.952000'),
	[CmdTransTime] [datetime] NULL,
	[CmdOverTime] [datetime] NULL,
	[CmdReturn] [int] NULL,
	[CmdReturnContent] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[CmdImmediately] [bit] NOT NULL CONSTRAINT [default_value_devcmds_bak_CmdImmediately]  DEFAULT ('False'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[devlog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SN_id] [int] NULL,
	[OP] [nvarchar](40) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [default_value_devlog_OP]  DEFAULT ('TRANSACT'),
	[Object] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Cnt] [int] NOT NULL CONSTRAINT [default_value_devlog_Cnt]  DEFAULT ('1'),
	[ECnt] [int] NOT NULL CONSTRAINT [default_value_devlog_ECnt]  DEFAULT ('0'),
	[OpTime] [datetime] NOT NULL CONSTRAINT [default_value_devlog_OpTime]  DEFAULT ('2011-07-15 16:06:23.952000'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[django_content_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[app_label] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[model] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[app_label] ASC,
	[model] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[django_session](
	[session_key] [nvarchar](40) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[session_data] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[expire_date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[session_key] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[EmOpLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [int] NOT NULL,
	[OperateTime] [datetime] NOT NULL,
	[manipulationID] [int] NULL,
	[Params1] [int] NULL,
	[Params2] [int] NULL,
	[Params3] [int] NULL,
	[Params4] [int] NULL,
	[SensorId] [nvarchar](5) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[empitemdefine](
	[ItemName] [nvarchar](100) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ItemType] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Author_id] [int] NULL,
	[ItemValue] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Published] [smallint] NULL CONSTRAINT [default_value_empitemdefine_Published]  DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[ItemName] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[EXCNOTES](
	[USERID] [int] NULL,
	[ATTDATE] [datetime] NULL,
	[NOTES] [nvarchar](200) COLLATE Chinese_PRC_CI_AS NULL
) ON [PRIMARY]
;

CREATE TABLE [dbo].[FaceTemp](
	[TEMPLATEID] [int] IDENTITY(1,1) NOT NULL,
	[USERNO] [nvarchar](24) COLLATE Chinese_PRC_CI_AS NULL,
	[SIZE] [int] NULL DEFAULT ((0)),
	[pin] [int] NULL DEFAULT ((0)),
	[FACEID] [int] NULL DEFAULT ((0)),
	[VALID] [int] NULL DEFAULT ((0)),
	[RESERVE] [int] NULL DEFAULT ((0)),
	[ACTIVETIME] [int] NULL DEFAULT ((0)),
	[VFCOUNT] [int] NULL DEFAULT ((0)),
	[TEMPLATE] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[TEMPLATEID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

CREATE TABLE [dbo].[HOLIDAYS](
	[HOLIDAYID] [int] NOT NULL,
	[HOLIDAYNAME] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[HOLIDAYYEAR] [smallint] NULL,
	[HOLIDAYMONTH] [smallint] NULL,
	[HOLIDAYDAY] [smallint] NULL,
	[STARTTIME] [datetime] NULL,
	[DURATION] [smallint] NULL,
	[HOLIDAYTYPE] [smallint] NULL DEFAULT ((1)),
	[XINBIE] [nvarchar](4) COLLATE Chinese_PRC_CI_AS NULL,
	[MINZU] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[DeptID] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[HOLIDAYID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[iclock_dstime](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL DEFAULT ('0'),
	[dst_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[mode] [int] NULL DEFAULT ('0'),
	[start_time] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[end_time] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[dst_name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[iclock_oplog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SN] [int] NULL,
	[admin] [int] NOT NULL DEFAULT ('0'),
	[OP] [int] NOT NULL DEFAULT ('0'),
	[OPTime] [datetime] NULL,
	[Object] [int] NULL,
	[Param1] [int] NULL,
	[Param2] [int] NULL,
	[Param3] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[SN] ASC,
	[OPTime] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[iclock_testdata](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL DEFAULT ('0'),
	[dept_id] [int] NULL,
	[area_id] [int] NULL DEFAULT ('1'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[iclock_testdata_admin_area](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[testdata_id] [int] NULL,
	[area_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[testdata_id] ASC,
	[area_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[iclock_testdata_admin_dept](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[testdata_id] [int] NULL,
	[department_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[testdata_id] ASC,
	[department_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

Create Table LeaveClass(
  LeaveId INT Identity(1,1) not null primary key,
  LeaveName VARCHAR(20) not null,                
  MinUnit float not null default 1,              
  Unit smallint not null default 1,              
  RemaindProc smallint not null default 1,     
  RemaindCount smallint not null default 1,    
  ReportSymbol varchar(4) not null default '-',
  Deduct float not null default 0,             
  Color int not null default 0,
  Classify SMALLINT NOT null default 0)
;

CREATE TABLE [dbo].[LeaveClass1](
	[LeaveId] [int] IDENTITY(1,1) NOT NULL primary key,
	[LeaveName] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[MinUnit] [float] NOT NULL DEFAULT ((1)),
	[Unit] [smallint] NOT NULL DEFAULT ((0)),
	[RemaindProc] [smallint] NOT NULL DEFAULT ((2)),
	[RemaindCount] [smallint] NOT NULL DEFAULT ((1)),
	[ReportSymbol] [nvarchar](4) COLLATE Chinese_PRC_CI_AS NOT NULL DEFAULT ('_'),
	[Deduct] [float] NOT NULL DEFAULT ((0)),
	[LeaveType] [smallint] NOT NULL DEFAULT ((0)),
	[Color] [int] NOT NULL DEFAULT ((0)),
	[Classify] [smallint] NOT NULL DEFAULT ((0)),
	[Calc] [nvarchar](max) COLLATE Chinese_PRC_CI_AS NULL,
	[Code] [nvarchar](16) COLLATE Chinese_PRC_CI_AS NULL
) ON [PRIMARY]
;

CREATE TABLE [dbo].[Machines](
	[ID] [int] IDENTITY(1,1) NOT NULL primary key,
	[MachineAlias] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[ConnectType] [int] NULL,
	[IP] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[SerialPort] [int] NULL DEFAULT ((1)),
	[Port] [int] NULL DEFAULT ('4370'),
	[Baudrate] [int] NULL,
	[MachineNumber] [int] NOT NULL DEFAULT ((1)),
	[IsHost] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL DEFAULT ('True'),
	[CommPassword] [nvarchar](12) COLLATE Chinese_PRC_CI_AS NULL,
	[UILanguage] [smallint] NULL DEFAULT ((-1)),
	[DateFormat] [smallint] NULL DEFAULT ((-1)),
	[InOutRecordWarn] [smallint] NULL DEFAULT ((-1)),
	[Idle] [smallint] NULL DEFAULT ((-1)),
	[Voice] [smallint] NULL DEFAULT ((-1)),
	[managercount] [smallint] NULL DEFAULT ((-1)),
	[usercount] [smallint] NULL DEFAULT ((-1)),
	[fingercount] [smallint] NULL DEFAULT ((-1)),
	[SecretCount] [smallint] NULL DEFAULT ((-1)),
	[FirmwareVersion] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[ProductType] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[LockControl] [smallint] NULL DEFAULT ((-1)),
	[Purpose] [smallint] NULL,
	[ProduceKind] [int] NULL,
	[sn] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[PhotoStamp] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[IsIfChangeConfigServer2] [int] NULL,
	[pushver] [int] NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL DEFAULT ((0)),
	[device_type] [int] NULL,
	[last_activity] [datetime] NULL,
	[trans_times] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[TransInterval] [int] NULL,
	[log_stamp] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[oplog_stamp] [image] NULL,
	[photo_stamp] [image] NULL,
	[UpdateDB] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ('1111101000'),
	[device_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[transaction_count] [int] NULL,
	[main_time] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[max_user_count] [int] NULL,
	[max_finger_count] [int] NULL,
	[max_attlog_count] [int] NULL,
	[alg_ver] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[flash_size] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[free_flash_size] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[language] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[lng_encode] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ('gb2312'),
	[volume] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[is_tft] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[platform] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[brightness] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[oem_vendor] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[city] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[AccFun] [int] NULL DEFAULT ('0'),
	[TZAdj] [int] NULL DEFAULT ('8'),
	[comm_type] [int] NULL,
	[agent_ipaddress] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[subnet_mask] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[gateway] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[area_id] [int] NULL,
	[acpanel_type] [int] NULL DEFAULT ('2'),
	[sync_time] [bit] NOT NULL DEFAULT ('True'),
	[four_to_two] [bit] NOT NULL DEFAULT ('False'),
	[video_login] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[fp_mthreshold] [int] NULL,
	[Fpversion] [int] NULL,
	[max_comm_size] [int] NULL CONSTRAINT [default_value_iclock_max_comm_size]  DEFAULT ('40'),
	[max_comm_count] [int] NULL CONSTRAINT [default_value_iclock_max_comm_count]  DEFAULT ('20'),
	[realtime] [bit] NOT NULL CONSTRAINT [default_value_iclock_realtime]  DEFAULT ('True'),
	[delay] [int] NULL CONSTRAINT [default_value_iclock_delay]  DEFAULT ('10'),
	[encrypt] [bit] NOT NULL CONSTRAINT [default_value_iclock_encrypt]  DEFAULT ('False'),
	[dstime_id] [int] NULL,
	[door_count] [int] NULL,
	[reader_count] [int] NULL,
	[aux_in_count] [int] NULL,
	[aux_out_count] [int] NULL,
	[IsOnlyRFMachine] [int] NULL,
	[alias] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[ipaddress] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[com_port] [smallint] NULL,
	[com_address] [smallint] NULL DEFAULT ('1')
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

CREATE TABLE [dbo].[NUM_RUN](
	[NUM_RUNID] [int] IDENTITY(1,1) NOT NULL,
	[OLDID] [int] NULL DEFAULT ((-1)),
	[NAME] [varchar](30) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[STARTDATE] [datetime] NULL DEFAULT ('2000-1-1'),
	[ENDDATE] [datetime] NULL DEFAULT ('2099-12-31'),
	[CYLE] [smallint] NULL DEFAULT ((1)),
	[UNITS] [smallint] NULL DEFAULT ((1)),
 CONSTRAINT [NUMID] PRIMARY KEY CLUSTERED 
(
	[NUM_RUNID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[NUM_RUN_DEIL](
	[NUM_RUNID] [smallint] NOT NULL,
	[STARTTIME] [datetime] NOT NULL,
	[ENDTIME] [datetime] NULL,
	[SDAYS] [smallint] NOT NULL,
	[EDAYS] [smallint] NULL,
	[SCHCLASSID] [int] NULL DEFAULT ((-1)),
 CONSTRAINT [NUMID2] PRIMARY KEY CLUSTERED 
(
	[NUM_RUNID] ASC,
	[SDAYS] ASC,
	[STARTTIME] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[operatecmds](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL DEFAULT ('0'),
	[Author_id] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[CmdContent] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[CmdCommitTime] [datetime] NULL DEFAULT ('2011-07-15 16:06:23.608000'),
	[commit_time] [datetime] NULL,
	[CmdReturn] [int] NULL,
	[process_count] [int] NULL DEFAULT ('0'),
	[success_flag] [int] NULL DEFAULT ('1'),
	[receive_data] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[cmm_type] [int] NULL,
	[cmm_system] [int] NULL DEFAULT ('1'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[personnel_area](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [smallint] NOT NULL CONSTRAINT [default_value_personnel_area_status]  DEFAULT ('0'),
	[areaid] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[areaname] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[parent_id] [int] NULL DEFAULT ((0)),
	[remark] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[personnel_cardtype](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[cardtypecode] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[cardtypename] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[personnel_empchange](
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[changeno] [int] IDENTITY(1,1) NOT NULL,
	[UserID_id] [int] NULL,
	[changedate] [datetime] NULL DEFAULT ('2011-07-15 16:06:23.936000'),
	[changepostion] [int] NULL,
	[oldvalue] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[newvalue] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[changereason] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[isvalid] [bit] NOT NULL DEFAULT ('0'),
	[approvalstatus] [int] NULL DEFAULT ('2'),
	[remark] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[changeno] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[personnel_issuecard](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[UserID_id] [int] NULL,
	[cardno] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[effectivenessdate] [datetime] NULL,
	[isvalid] [bit] NOT NULL DEFAULT ('1'),
	[cardpwd] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[failuredate] [datetime] NULL,
	[cardstatus] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ('1'),
	[issuedate] [datetime] NULL DEFAULT ('2011-07-15'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[personnel_leavelog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[UserID_id] [int] NULL,
	[leavedate] [datetime] NULL,
	[leavetype] [int] NULL,
	[reason] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[isReturnTools] [bit] NOT NULL DEFAULT ('True'),
	[isReturnClothes] [bit] NOT NULL DEFAULT ('True'),
	[isReturnCard] [bit] NOT NULL DEFAULT ('True'),
	[isClassAtt] [bit] NOT NULL DEFAULT ('0'),
	[isClassAccess] [bit] NOT NULL DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[ReportItem](
	[RIID] [int] IDENTITY(1,1) NOT NULL,
	[RIIndex] [int] NULL,
	[ShowIt] [smallint] NULL,
	[RIName] [varchar](12) COLLATE Chinese_PRC_CI_AS NULL,
	[UnitName] [varchar](6) COLLATE Chinese_PRC_CI_AS NULL,
	[Formula] [image] NOT NULL,
	[CalcBySchClass] [smallint] NULL,
	[StatisticMethod] [smallint] NULL,
	[CalcLast] [smallint] NULL,
	[Notes] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[RIID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

CREATE TABLE [dbo].[SchClass](
	[schClassid] [int] IDENTITY(1,1) NOT NULL,
	[schName] [varchar](20) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[LateMinutes] [int] NULL,
	[EarlyMinutes] [int] NULL,
	[CheckIn] [int] NULL DEFAULT ((1)),
	[CheckOut] [int] NULL DEFAULT ((1)),
	[CheckInTime1] [datetime] NULL,
	[CheckInTime2] [datetime] NULL,
	[CheckOutTime1] [datetime] NULL,
	[CheckOutTime2] [datetime] NULL,
	[Color] [int] NULL DEFAULT ((16715535)),
	[AutoBind] [smallint] NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED 
(
	[schClassid] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[SECURITYDETAILS](
	[SECURITYDETAILID] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [smallint] NULL,
	[DEPTID] [smallint] NULL,
	[SCHEDULE] [smallint] NULL,
	[USERINFO] [smallint] NULL,
	[ENROLLFINGERS] [smallint] NULL,
	[REPORTVIEW] [smallint] NULL,
	[REPORT] [varchar](10) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [NAMEID2] PRIMARY KEY CLUSTERED 
(
	[SECURITYDETAILID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[ServerLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EVENT] [varchar](30) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[USERID] [int] NOT NULL,
	[EnrollNumber] [varchar](30) COLLATE Chinese_PRC_CI_AS NULL,
	[parameter] [smallint] NULL,
	[EVENTTIME] [datetime] NOT NULL DEFAULT (getdate()),
	[SENSORID] [varchar](5) COLLATE Chinese_PRC_CI_AS NULL,
	[OPERATOR] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[SHIFT](
	[SHIFTID] [int] IDENTITY(1,1) NOT NULL,
	[NAME] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[USHIFTID] [int] NULL DEFAULT ((-1)),
	[STARTDATE] [datetime] NOT NULL DEFAULT ('1900-1-1'),
	[ENDDATE] [datetime] NULL DEFAULT ('1900-12-31'),
	[RUNNUM] [smallint] NULL DEFAULT ((0)),
	[SCH1] [int] NULL DEFAULT ((0)),
	[SCH2] [int] NULL DEFAULT ((0)),
	[SCH3] [int] NULL DEFAULT ((0)),
	[SCH4] [int] NULL DEFAULT ((0)),
	[SCH5] [int] NULL DEFAULT ((0)),
	[SCH6] [int] NULL DEFAULT ((0)),
	[SCH7] [int] NULL DEFAULT ((0)),
	[SCH8] [int] NULL DEFAULT ((0)),
	[SCH9] [int] NULL DEFAULT ((0)),
	[SCH10] [int] NULL DEFAULT ((0)),
	[SCH11] [int] NULL DEFAULT ((0)),
	[SCH12] [int] NULL DEFAULT ((0)),
	[CYCLE] [smallint] NULL DEFAULT ((0)),
	[UNITS] [smallint] NULL DEFAULT ((0)),
 CONSTRAINT [SHIFTS] PRIMARY KEY CLUSTERED 
(
	[SHIFTID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[SystemLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Operator] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[LogTime] [datetime] NULL DEFAULT (getdate()),
	[MachineAlias] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[LogTag] [smallint] NULL,
	[LogDescr] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[TBKEY](
	[PreName] [nvarchar](12) COLLATE Chinese_PRC_CI_AS NULL,
	[ONEKEY] [int] NULL
) ON [PRIMARY]


CREATE TABLE [dbo].[TBSMSALLOT](
	[REFERENCE] [int] NOT NULL,
	[SMSREF] [int] NOT NULL,
	[USERREF] [int] NOT NULL,
	[GENTM] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [pk_tbsmsallog] PRIMARY KEY CLUSTERED 
(
	[SMSREF] ASC,
	[USERREF] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[TBSMSINFO](
	[REFERENCE] [int] NOT NULL,
	[SMSID] [varchar](16) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[SMSINDEX] [int] NOT NULL,
	[SMSTYPE] [int] NULL,
	[SMSCONTENT] [varchar](200) COLLATE Chinese_PRC_CI_AS NULL,
	[SMSSTARTTM] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[SMSTMLENG] [int] NULL,
	[GENTM] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [pk_TBSMSINFO] PRIMARY KEY CLUSTERED 
(
	[REFERENCE] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[TEMPLATE](
	[TEMPLATEID] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [int] NOT NULL,
	[FINGERID] [int] NOT NULL DEFAULT ('0'),
	[TEMPLATE] [image] NULL,
	[TEMPLATE2] [image] NULL,
	[BITMAPPICTURE] [image] NULL,
	[BITMAPPICTURE2] [image] NULL,
	[BITMAPPICTURE3] [image] NULL,
	[BITMAPPICTURE4] [image] NULL,
	[USETYPE] [smallint] NULL,
	[EMACHINENUM] [nvarchar](3) COLLATE Chinese_PRC_CI_AS NULL,
	[TEMPLATE1] [image] NULL,
	[Flag] [smallint] NULL,
	[DivisionFP] [smallint] NULL,
	[TEMPLATE4] [image] NULL,
	[TEMPLATE3] [image] NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[Valid] [int] NULL DEFAULT ('1'),
	[Fpversion] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[bio_type] [int] NULL DEFAULT ('0'),
	[SN] [int] NULL,
	[UTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[TEMPLATEID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

CREATE TABLE [dbo].[USER_OF_RUN](
	[USERID] [int] NOT NULL,
	[NUM_OF_RUN_ID] [int] NOT NULL,
	[STARTDATE] [datetime] NOT NULL DEFAULT ('1900-1-1'),
	[ENDDATE] [datetime] NOT NULL DEFAULT ('2099-12-31'),
	[ISNOTOF_RUN] [int] NULL DEFAULT ((0)),
	[ORDER_RUN] [int] NULL,
 CONSTRAINT [USER_ST_NUM] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC,
	[NUM_OF_RUN_ID] ASC,
	[STARTDATE] ASC,
	[ENDDATE] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[USER_SPEDAY](
	[USERID] [int] NOT NULL,
	[STARTSPECDAY] [datetime] NOT NULL DEFAULT ('1900-1-1'),
	[ENDSPECDAY] [datetime] NULL DEFAULT ('2099-12-31'),
	[DATEID] [smallint] NOT NULL DEFAULT ((-1)),
	[YUANYING] [varchar](200) COLLATE Chinese_PRC_CI_AS NULL,
	[DATE] [datetime] NULL,
 CONSTRAINT [USER_SEP] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC,
	[STARTSPECDAY] ASC,
	[DATEID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[USER_TEMP_SCH](
	[USERID] [int] NOT NULL,
	[COMETIME] [datetime] NOT NULL,
	[LEAVETIME] [datetime] NOT NULL,
	[OVERTIME] [int] NOT NULL DEFAULT ((0)),
	[TYPE] [smallint] NULL DEFAULT ((0)),
	[FLAG] [smallint] NULL DEFAULT ((1)),
	[SCHCLASSID] [int] NULL DEFAULT ((-1)),
 CONSTRAINT [USER_TEMP] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC,
	[COMETIME] ASC,
	[LEAVETIME] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[UserACMachines](
	[UserID] [int] NOT NULL,
	[Deviceid] [int] NOT NULL,
 CONSTRAINT [UserACMachinesPK] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[Deviceid] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[UserACPrivilege](
	[UserID] [int] NOT NULL,
	[ACGroupID] [smallint] NOT NULL,
	[IsUseGroup] [bit] NOT NULL,
	[TimeZone1] [smallint] NULL,
	[TimeZone2] [smallint] NULL,
	[TimeZone3] [smallint] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[USERINFO](
	[USERID] [int] IDENTITY(1,1) NOT NULL,
	[Badgenumber] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[SSN] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[Name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Gender] [nvarchar](8) COLLATE Chinese_PRC_CI_AS NULL,
	[TITLE] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[PAGER] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[BIRTHDAY] [datetime] NULL,
	[HIREDDAY] [datetime] NULL,
	[street] [nvarchar](80) COLLATE Chinese_PRC_CI_AS NULL,
	[CITY] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[STATE] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[ZIP] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[OPHONE] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[FPHONE] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[VERIFICATIONMETHOD] [smallint] NULL,
	[DEFAULTDEPTID] [int] NULL DEFAULT ((1)),
	[SECURITYFLAGS] [smallint] NULL,
	[ATT] [smallint] NOT NULL DEFAULT ((1)),
	[INLATE] [smallint] NOT NULL DEFAULT ((1)),
	[OUTEARLY] [smallint] NOT NULL DEFAULT ((1)),
	[OVERTIME] [smallint] NOT NULL DEFAULT ((1)),
	[SEP] [smallint] NOT NULL DEFAULT ((1)),
	[HOLIDAY] [smallint] NOT NULL DEFAULT ((1)),
	[MINZU] [nvarchar](8) COLLATE Chinese_PRC_CI_AS NULL,
	[PASSWORD] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[LUNCHDURATION] [smallint] NULL,
	[PHOTO] [image] NULL,
	[mverifypass] [nvarchar](10) COLLATE Chinese_PRC_CI_AS NULL,
	[Notes] [image] NULL,
	[privilege] [int] NULL DEFAULT ((0)),
	[InheritDeptSch] [smallint] NULL,
	[InheritDeptSchClass] [smallint] NULL,
	[AutoSchPlan] [smallint] NULL DEFAULT ((1)),
	[MinAutoSchInterval] [int] NULL DEFAULT ((24)),
	[RegisterOT] [smallint] NULL DEFAULT ((1)),
	[InheritDeptRule] [smallint] NULL,
	[EMPRIVILEGE] [smallint] NULL,
	[CardNo] [nvarchar](20) COLLATE Chinese_PRC_CI_AS NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL DEFAULT ('0'),
	[lastname] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[AccGroup] [int] NULL,
	[TimeZones] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[identitycard] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[UTime] [datetime] NULL,
	[Education] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[OffDuty] [int] NULL,
	[DelTag] [int] NULL,
	[morecard_group_id] [int] NULL,
	[set_valid_time] [bit] NULL,
	[acc_startdate] [datetime] NULL,
	[acc_enddate] [datetime] NULL,
	[birthplace] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Political] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[contry] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[hiretype] [int] NULL,
	[email] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[firedate] [datetime] NULL,
	[isatt] [bit] NULL,
	[homeaddress] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[emptype] [int] NULL,
	[bankcode1] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[bankcode2] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[isblacklist] [int] NULL,
	[Iuser1] [int] NULL,
	[Iuser2] [int] NULL,
	[Iuser3] [int] NULL,
	[Iuser4] [int] NULL,
	[Iuser5] [int] NULL,
	[Cuser1] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Cuser2] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Cuser3] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Cuser4] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Cuser5] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[Duser1] [datetime] NULL,
	[Duser2] [datetime] NULL,
	[Duser3] [datetime] NULL,
	[Duser4] [datetime] NULL,
	[Duser5] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[USERID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
;

CREATE TABLE [dbo].[userinfo_attarea](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[employee_id] [int] NOT NULL,
	[area_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[employee_id] ASC,
	[area_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[UsersMachines](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [int] NOT NULL,
	[DEVICEID] [int] NOT NULL,
 CONSTRAINT [UsersMachinesPK] PRIMARY KEY CLUSTERED 
(
	[USERID] ASC,
	[DEVICEID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[UserUpdates](
	[UpdateId] [int] IDENTITY(1,1) NOT NULL,
	[BadgeNumber] [varchar](20) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[UpdateId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[UserUsedSClasses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SchId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[UserId] ASC,
	[SchId] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[worktable_groupmsg](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL DEFAULT ('0'),
	[msgtype_id] [int] NULL,
	[group_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[worktable_instantmsg](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[monitor last time] [datetime] NULL,
	[msgtype_id] [int] NULL,
	[content] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[worktable_msgtype](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] NULL,
	[msgtype_name] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[msgtype_value] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[msg_keep_time] [int] NULL DEFAULT ('1'),
	[msg_ahead_remind] [int] NULL DEFAULT ('0'),
	[type] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ('0'),
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

CREATE TABLE [dbo].[worktable_usrmsg](
	[id] [int] IDENTITY(1,1) NOT NULL primary key,
	[change_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[change_time] [datetime] NULL,
	[create_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[create_time] [datetime] NULL,
	[delete_operator] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[delete_time] [datetime] NULL,
	[status] [int] DEFAULT ('0'),
	[user_id] [int] NULL,
	[msg_id] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL
) ON [PRIMARY]
;

CREATE TABLE [dbo].[ZKAttendanceMonthStatistics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[USERID] [int] NULL,
	[PortalSite] [int] NULL,
	[Department] [int] NULL,
	[Employee] [int] NULL,
	[Year] [int] NULL,
	[Month] [int] NULL,
	[YingDao] [decimal](9, 1) NULL,
	[ShiDao] [decimal](9, 1) NULL,
	[ChiDao] [decimal](9, 1) NULL,
	[ZaoTui] [decimal](9, 1) NULL,
	[KuangGong] [decimal](9, 1) NULL,
	[JiaBan] [decimal](9, 1) NULL,
	[WaiChu] [decimal](9, 1) NULL,
	[YinGongWaiChu] [decimal](9, 1) NULL,
	[GongZuoShiJian] [decimal](9, 1) NULL,
	[YingQian] [decimal](9, 1) NULL,
	[QianDao] [decimal](9, 1) NULL,
	[QianTui] [decimal](9, 1) NULL,
	[WeiQianDao] [decimal](9, 1) NULL,
	[WeiQianTui] [decimal](9, 1) NULL,
	[QingJia] [decimal](9, 1) NULL,
	[PingRi] [decimal](9, 1) NULL,
	[ZhouMo] [decimal](9, 1) NULL,
	[JieJiaRi] [decimal](9, 1) NULL,
	[ChuQinShiJian] [decimal](9, 1) NULL,
	[PingRiJiaBan] [decimal](9, 1) NULL,
	[ZhouMoJiaBan] [decimal](9, 1) NULL,
	[JieJiaRiJiaBan] [decimal](9, 1) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
;

insert into LeaveClass(LeaveName, Unit, ReportSymbol, Color) values('病假', 1, 'B', 3398744);
insert into LeaveClass(LeaveName, Unit, ReportSymbol, Color) values('事假', 1, 'S', 8421631);
insert into LeaveClass(LeaveName, Unit, ReportSymbol, Color) values('探亲假', 1, 'T', 16744576);

insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('公出', 0.5, 3, 1, 1, 'G', 3, 'if(AttItem(LeaveType1)=999,AttItem(LeaveTime1),0)+if(AttItem(LeaveType2)=999,AttItem(LeaveTime2),0)+if(AttItem(LeaveType3)=999,AttItem(LeaveTime3),0)+if(AttItem(LeaveType4)=999,AttItem(LeaveTime4),0)+if(AttItem(LeaveType5)=999,AttItem(LeaveTime5),0)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType)
  values('正常', 0.5, 3, 1, 0, ' ', 3);
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('迟到', 10, 2, 2, 1, '>', 3, 'AttItem(minLater)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('早退', 10, 2, 2, 1, '<', 3, 'AttItem(minEarly)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('请假', 1, 1, 1, 1, '假', 3, 
  'if((AttItem(LeaveType1)>0) and (AttItem(LeaveType1)<999),AttItem(LeaveTime1),0)+if((AttItem(LeaveType2)>0) and (AttItem(LeaveType2)<999),AttItem(LeaveTime2),0)+if((AttItem(LeaveType3)>0) and (AttItem(LeaveType3)<999),AttItem(LeaveTime3),0)+if((AttItem(LeaveType4)>0) and (AttItem(LeaveType4)<999),AttItem(LeaveTime4),0)+if((AttItem(LeaveType5)>0) and (AttItem(LeaveType5)<999),AttItem(LeaveTime5),0)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('旷工', 0.5, 3, 1, 0, '旷', 3, 'AttItem(MinAbsent)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('加班', 1, 1, 1, 1, '+', 3, 'AttItem(MinOverTime)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('节日加班', 1, 1, 0, 1, '=', 0, 'if(HolidayId(1)=1, AttItem(MinOverTime),0)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType)
  values('休息日', 0.5, 3, 2, 1, '-', 2);
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('未签到', 1, 4, 2, 1, '[', 2, 
  'If(AttItem(CheckIn)=null,If(AttItem(OnDuty)=null,0,if(((AttItem(LeaveStart1)=null) or (AttItem(LeaveStart1)>AttItem(OnDuty))) and AttItem(DutyOn),1,0)),0)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType, Calc)
  values('未签退', 1, 4, 2, 1, ']', 2, 
  'If(AttItem(CheckOut)=null,If(AttItem(OffDuty)=null,0,if((AttItem(LeaveEnd1)=null) or (AttItem(LeaveEnd1)<AttItem(OffDuty)),if((AttItem(LeaveEnd2)=null) or (AttItem(LeaveEnd2)<AttItem(OffDuty)),if(((AttItem(LeaveEnd3)=null) or (AttItem(LeaveEnd3)<AttItem(OffDuty))) and AttItem(DutyOff),1,0),0),0)),0)');
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType)
  values('离岗未签到', 1, 4, 2, 1, '{', 6);
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType)
  values('离岗未签退', 1, 4, 2, 1, '}', 6);
insert into LeaveClass1(LeaveName, MinUnit, Unit, RemaindProc,
  RemaindCount, ReportSymbol, LeaveType)
  values('离岗', 1, 1, 2, 1, 'L', 3);
  
insert into AttParam(ParaName,ParaValue) values('MinsEarly',5);
insert into AttParam(ParaName,ParaValue) values('MinsLate',10);
insert into AttParam(ParaName,ParaValue) values('MinsNoBreakIn',60);
insert into AttParam(ParaName,ParaValue) values('MinsNoBreakOut',60);
insert into AttParam(ParaName,ParaValue) values('MinsNoIn',60);
insert into AttParam(ParaName,ParaValue) values('MinsNoLeave',60);
insert into AttParam(ParaName,ParaValue) values('MinsNotOverTime',60);
insert into AttParam(ParaName,ParaValue) values('MinsWorkDay',420);
insert into AttParam(ParaName,ParaValue) values('NoBreakIn',1012);
insert into AttParam(ParaName,ParaValue) values('NoBreakOut',1012);
insert into AttParam(ParaName,ParaValue) values('NoIn',1001);
insert into AttParam(ParaName,ParaValue) values('NoLeave',1002);
insert into AttParam(ParaName,ParaValue) values('OutOverTime',0);
insert into AttParam(ParaName,ParaValue) values('TwoDay',0);
insert into AttParam(ParaName,ParaValue) values('CheckInColor',16777151);
insert into AttParam(ParaName,ParaValue) values('CheckOutColor',12910591);
insert into AttParam(ParaName,ParaValue) values('DBVersion',167);





insert into Auth_User(username,password,Status,RoleID,Remark) values('admin','admin',1,0,'');

insert into departments(deptname,supdeptid,code) values('公司名称',0,'1');

insert into personnel_area(areaid,areaname,parent_id) values('1','区域名称',0);

insert into acc_timeseg(timeseg_name,sunday_start1,sunday_end1,sunday_start2,sunday_end2,sunday_start3,sunday_end3,monday_start1,monday_end1,monday_start2,monday_end2,monday_start3,monday_end3,tuesday_start1,tuesday_end1,tuesday_start2,tuesday_end2,tuesday_start3,tuesday_end3,wednesday_start1,wednesday_end1,wednesday_start2,wednesday_end2,wednesday_start3,wednesday_end3,thursday_start1,thursday_end1,thursday_start2,thursday_end2,thursday_start3,thursday_end3,friday_start1,friday_end1,friday_start2,friday_end2,friday_start3,friday_end3,saturday_start1,saturday_end1,saturday_start2,saturday_end2,saturday_start3,saturday_end3,holidaytype1_start1,holidaytype1_end1,holidaytype1_start2,holidaytype1_end2,holidaytype1_start3,holidaytype1_end3,holidaytype2_start1,holidaytype2_end1,holidaytype2_start2,holidaytype2_end2,holidaytype2_start3,holidaytype2_end3,holidaytype3_start1,holidaytype3_end1,holidaytype3_start2,holidaytype3_end2,holidaytype3_start3,holidaytype3_end3) values('24小时通行','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00','00:00','23:59','00:00','00:00','00:00','00:00');

