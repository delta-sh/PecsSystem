﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Web.Caching;
using System.Xml;
using Ext.Net;
using Delta.PECS.WebCSC.BLL;
using Delta.PECS.WebCSC.Model;

namespace Delta.PECS.WebCSC.Site {
    [DirectMethodProxyID(IDMode = DirectMethodProxyIDMode.Alias, Alias = "MajorIncidentReport")]
    public partial class MajorIncidentReport : PageBase {
        protected void Page_Load(object sender, EventArgs e) {
            if (!X.IsAjaxRequest) {
                ResourceManager.GetInstance().RegisterIcon(Icon.House);
                ResourceManager.GetInstance().RegisterIcon(Icon.Building);
                ResourceManager.GetInstance().RegisterIcon(Icon.TextListBullets);
                ResourceManager.GetInstance().RegisterIcon(Icon.TagBlue);
            }

            if (!IsPostBack && !X.IsAjaxRequest) {
                BeginFromDate.Text = WebUtility.GetDateString(DateTime.Today.AddMonths(-1));
                BeginToDate.Text = WebUtility.GetDateString(DateTime.Now);
            }
        }

        /// <summary>
        /// Init Alarm Interval Count Tree Nodes.
        /// </summary>
        [DirectMethod(Timeout = 300000)]
        public string InitCountItemTreeNodes() {
            try {
                var root = new Ext.Net.TreeNode();
                root.Text = "动力环境监控中心系统";
                root.NodeID = "-1&-1";
                root.Icon = Icon.House;
                root.Expanded = true;
                root.SingleClickExpand = true;
                CountItemTreePanel.Root.Clear();
                CountItemTreePanel.Root.Add(root);

                var userData = UserData;
                foreach (var lscUser in userData.LscUsers) {
                    if (lscUser.Group == null) { continue; }
                    var node = new Ext.Net.TreeNode();
                    node.Text = lscUser.LscName;
                    node.NodeID = String.Format("{0}&{1}&{2}&{3}&{4}", lscUser.Group.LscID, lscUser.Group.GroupID, 0, (Int32)EnmNodeType.LSC, String.Empty);
                    node.Icon = Icon.House;
                    node.SingleClickExpand = true;
                    node.Checked = ThreeStateBool.False;
                    root.Nodes.Add(node);

                    CountItemTreeNodesLoaded(node, 0, lscUser.Group);
                }
                return CountItemTreePanel.Root.ToJson();
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
            return String.Empty;
        }

        /// <summary>
        /// Loaded Alarm Interval Count Tree Nodes.
        /// </summary>
        /// <param name="pnode">Parent Node</param>
        /// <param name="pId">Parent Id</param>
        /// <param name="group">Lsc Group</param>
        private void CountItemTreeNodesLoaded(Ext.Net.TreeNode pnode, int pId, GroupInfo group) {
            try {
                if (CountTypeComboBox.SelectedItem.Value.Equals("0")) { return; }
                var groupNodes = group.GroupNodes.FindAll(gti => { return gti.LastNodeID == pId; });
                foreach (var gti in groupNodes) {
                    if (gti.NodeType == EnmNodeType.Area) {
                        if (CountTypeComboBox.SelectedItem.Value.Equals("1")
                            && gti.Remark.Equals("3")) { continue; }

                        var node = new Ext.Net.TreeNode();
                        node.Text = gti.NodeName;
                        node.NodeID = String.Format("{0}&{1}&{2}&{3}&{4}", group.LscID, group.GroupID, gti.NodeID, (Int32)gti.NodeType, gti.Remark);
                        node.Icon = Icon.Building;
                        node.SingleClickExpand = true;
                        node.Checked = ThreeStateBool.False;
                        pnode.Nodes.Add(node);
                        CountItemTreeNodesLoaded(node, gti.NodeID, group);
                    }
                }
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
        }

        /// <summary>
        /// Init Alarm Name TreePanel
        /// </summary>
        [DirectMethod(Timeout = 300000)]
        public string InitAlarmNameTreePanel() {
            try {
                var root = new Ext.Net.TreeNode();
                root.Text = "告警类型";
                root.NodeID = "-1&-1";
                root.Icon = Icon.House;
                root.Leaf = false;
                root.Expanded = true;
                root.SingleClickExpand = true;
                AlarmNameTreePanel.Root.Clear();
                AlarmNameTreePanel.Root.Add(root);

                var dict = new BComboBox().GetAlarmDevs();
                if (dict != null && dict.Count > 0) {
                    foreach (var key in dict) {
                        var node = new AsyncTreeNode();
                        node.Text = key.Value;
                        node.NodeID = String.Format("0&{0}", key.Key);
                        node.Icon = Icon.TextListBullets;
                        node.Leaf = false;
                        node.Expanded = false;
                        node.SingleClickExpand = true;
                        root.Nodes.Add(node);
                    }
                }
                return AlarmNameTreePanel.Root.ToJson();
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
            return String.Empty;
        }

        /// <summary>
        /// Load Alarm Name Tree Nodes
        /// </summary>
        protected void AlarmNameLoaded(object sender, NodeLoadEventArgs e) {
            try {
                if (String.IsNullOrEmpty(e.NodeID)) { return; }
                var ids = WebUtility.ItemSplit(e.NodeID);
                if (ids.Length != 2) { return; }
                var type = Int32.Parse(ids[0]);
                var pId = Int32.Parse(ids[1]);

                if (type == 0) {
                    var dict = new BComboBox().GetAlarmLogics(pId);
                    if (dict != null && dict.Count > 0) {
                        foreach (var key in dict) {
                            var node = new AsyncTreeNode();
                            node.Text = key.Value;
                            node.NodeID = String.Format("1&{0}", key.Key);
                            node.Icon = Icon.TextListBullets;
                            node.Leaf = false;
                            node.SingleClickExpand = true;
                            e.Nodes.Add(node);
                        }
                    }
                } else if(type == 1) {
                    var values = new List<String>(AlarmNameDropDownField.Text.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                    var dict = new BComboBox().GetAlarmNames(pId);
                    if (dict != null && dict.Count > 0) {
                        foreach (var key in dict) {
                            var node = new Ext.Net.TreeNode();
                            node.Text = key.Value;
                            node.NodeID = key.Key;
                            node.Icon = Icon.TagBlue;
                            node.Leaf = true;
                            node.Checked = values.Any(v => v.Equals(node.NodeID)) ? ThreeStateBool.True : ThreeStateBool.False;
                            e.Nodes.Add(node);
                        }
                    }
                }
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
        }

        /// <summary>
        /// AlarmLevel ComboBox Refresh
        /// </summary>
        protected void OnAlarmLevelRefresh(object sender, StoreRefreshDataEventArgs e) {
            try {
                var data = new List<object>();
                var comboboxEntity = new BComboBox();
                var dict = comboboxEntity.GetAlarmLevels();
                if (dict != null && dict.Count > 0) {
                    foreach (var key in dict) {
                        data.Add(new {
                            Id = key.Key,
                            Name = key.Value
                        });
                    }
                }

                AlarmLevelStore.DataSource = data;
                AlarmLevelStore.DataBind();
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
        }

        /// <summary>
        /// Query Button Click
        /// </summary>
        protected void QueryBtn_Click(object sender, DirectEventArgs e) {
            try {
                string[] cols = null;
                var countType = CountTypeComboBox.SelectedItem.Value;
                if (countType.Equals("0")) {
                    cols = new string[] { "序号", "Lsc名称" };
                } else if (countType.Equals("1")) {
                    cols = new string[] { "序号", "Lsc名称", "地区" };
                } else if (countType.Equals("2")) {
                    cols = new string[] { "序号", "Lsc名称", "地区", "县市" };
                } else if (countType.Equals("3")) {
                    cols = new string[] { "序号", "Lsc名称", "地区", "县市", "局站" };
                } else { return; }

                var grid = MainGridPanel;
                var store = grid.GetStore();
                store.RemoveFields();
                grid.ColumnModel.Columns.Clear();
                for (int i = 0; i < cols.Length; i++) {
                    var dataIndex = String.Format("Data{0}", i);
                    store.AddField(new RecordField(dataIndex, RecordFieldType.String), false);

                    var col = new Column();
                    col.Header = cols[i];
                    col.DataIndex = dataIndex;
                    col.CustomConfig.Add(new ConfigItem("DblClickEnabled", "0", ParameterMode.Value));
                    col.Groupable = false;
                    col.Locked = true;
                    grid.ColumnModel.Columns.Add(col);
                }

                //Create Columns
                var columns = new String[] { "告警总量" , "重大故障总量", "重大故障占比" };
                var plen = cols.Length;
                var nlen = columns.Length;
                if (columns.Length > 0) {
                    for (int i = 0; i < nlen; i++) {
                        var dataIndex = String.Format("Data{0}", plen + i);
                        store.AddField(new RecordField(dataIndex, RecordFieldType.String), false);

                        var col = new Column();
                        col.Header = columns[i];
                        col.DataIndex = dataIndex;
                        col.CustomConfig.Add(new ConfigItem("DblClickEnabled", i == 0 || i == 1 ? "1" : "0", ParameterMode.Value));
                        col.Groupable = false;
                        grid.ColumnModel.Columns.Add(col);
                    }
                }

                store.ClearMeta();
                grid.Reconfigure();
                AddDataToCache();
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
        }

        /// <summary>
        /// Save Button Click
        /// </summary>
        protected void SaveBtn_Click(object sender, DirectEventArgs e) {
            try {
                var userData = UserData;
                var cacheKey = WebUtility.GetCacheKeyName(userData, "major-incident-source1");
                var data = HttpRuntime.Cache[cacheKey] as DataTable;
                if (data == null) {
                    WebUtility.ShowMessage(EnmErrType.Warning, "获取数据时发生错误，导出失败！");
                    return;
                }

                var colNames = e.ExtraParams["ColumnNames"];
                var names = new XmlDocument();
                names.LoadXml(colNames);
                var datas = new XmlDocument();
                var root = datas.CreateElement("records");
                datas.AppendChild(root);
                for (int i = 0; i < data.Rows.Count; i++) {
                    var parent_Node = datas.CreateElement("record");
                    root.AppendChild(parent_Node);

                    for (int j = 0; j < data.Columns.Count; j++) {
                        var element = datas.CreateElement(data.Columns[j].ColumnName);
                        element.InnerText = data.Rows[i][j].ToString();
                        parent_Node.AppendChild(element);
                    }
                }

                var fileName = "MajorIncidentReport.xls";
                var sheetName = "MajorIncidentReport";
                var title = "动力环境监控中心系统 重大故障统计报表";
                var subTitle = String.Format("值班员:{0}  日期:{1}", Page.User.Identity.Name, WebUtility.GetDateString(DateTime.Now));
                var xls = WebUtility.ExportDataToExcel(fileName, sheetName, title, subTitle, names, datas);
                if (xls != null) { xls.Send(); }
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
        }

        /// <summary>
        /// Main Grid Store Refresh
        /// </summary>
        protected void OnMainGridRefresh(object sender, StoreRefreshDataEventArgs e) {
            try {
                var start = Int32.Parse(e.Parameters["start"]);
                var limit = Int32.Parse(e.Parameters["limit"]);
                var end = start + limit;
                var data = new DataTable();

                var userData = UserData;
                var cacheKey = WebUtility.GetCacheKeyName(userData, "major-incident-source1");
                var source1 = HttpRuntime.Cache[cacheKey] as DataTable;
                if (source1 == null) { source1 = AddDataToCache(); }
                if (source1 != null && source1.Rows.Count > 0) {
                    data = source1.Clone();
                    if (end > source1.Rows.Count) { end = source1.Rows.Count; }
                    for (int i = start; i < end; i++) {
                        data.Rows.Add(source1.Rows[i].ItemArray);
                    }
                }

                e.Total = (source1 != null ? source1.Rows.Count : 0);
                MainGridStore.DataSource = data;
                MainGridStore.DataBind();
            } catch (Exception err) {
                WebUtility.WriteLog(EnmSysLogLevel.Error, EnmSysLogType.Exception, err.ToString(), Page.User.Identity.Name);
                WebUtility.ShowMessage(EnmErrType.Error, err.Message);
            }
        }

        /// <summary>
        /// Add Data To Cache
        /// </summary>
        private DataTable AddDataToCache() {
            var userData = UserData;
            var cacheKey1 = WebUtility.GetCacheKeyName(userData, "major-incident-source1");
            var cacheKey2 = WebUtility.GetCacheKeyName(userData, "major-incident-source2");
            HttpRuntime.Cache.Remove(cacheKey1);
            HttpRuntime.Cache.Remove(cacheKey2);

            if (String.IsNullOrEmpty(CountItemField.RawValue.ToString())) { return null; }
            var values = WebUtility.StringSplit(CountItemField.RawValue.ToString());
            var beginTime = Convert.ToDateTime(BeginFromDate.Text);
            var endTime = Convert.ToDateTime(BeginToDate.Text);
            var almIds = new Dictionary<Int32, String>();
            var levels = new Dictionary<Int32, String>();
            var levelLen = AlarmLevelMultiCombo.SelectedItems.Count;
            if (levelLen == 0) { return null; }
            for (int i = 0; i < levelLen; i++) {
                var key = Int32.Parse(AlarmLevelMultiCombo.SelectedItems[i].Value);
                var value = AlarmLevelMultiCombo.SelectedItems[i].Text;
                levels[key] = value;
            }
            
            var text = AlarmNameDropDownField.Text.Trim();
            if (text == "") { return null; }
            var names = WebUtility.StringSplit(text);
            for (int i = 0; i < names.Length; i++) {
                almIds[Int32.Parse(names[i])] = null;
            }

            var colcnt = MainGridPanel.ColumnModel.Columns.Count;
            var countType = CountTypeComboBox.SelectedItem.Value;
            var condition = new List<GroupTreeInfo>();
            var source1 = CreateCustomizeTable(colcnt);
            var source2 = new Dictionary<String, List<AlarmInfo>>();

            if (countType.Equals("0")) {
                #region Lsc
                foreach (var v in values) {
                    var ids = WebUtility.ItemSplit(v);
                    if (ids.Length != 5) { continue; }

                    var lscId = Int32.Parse(ids[0]);
                    var groupId = Int32.Parse(ids[1]);
                    var nodeId = Int32.Parse(ids[2]);
                    var nodeType = Int32.Parse(ids[3]);
                    var remark = ids[4];
                    var enmNodeType = Enum.IsDefined(typeof(EnmNodeType), nodeType) ? (EnmNodeType)nodeType : EnmNodeType.Null;
                    if (enmNodeType == EnmNodeType.LSC) {
                        condition.Add(new GroupTreeInfo() {
                            LscID = lscId,
                            GroupID = groupId,
                            NodeID = nodeId,
                            NodeType = enmNodeType,
                            Remark = remark
                        });
                    }
                }

                var ls = from c in condition
                         group c by new { c.LscID } into g
                         select new { g.Key.LscID };

                foreach (var l in ls) {
                    var lscUser = userData.LscUsers.Find(lu => { return lu.LscID == l.LscID; });
                    if (lscUser == null) { continue; }

                    var dr1 = source1.NewRow();
                    dr1[1] = lscUser.LscName;

                    var total = WebUtility.GetUserAlarms(userData).FindAll(alarm => alarm.LscID == lscUser.LscID && alarm.StartTime >= beginTime && alarm.StartTime <= endTime);
                    total.AddRange(new BAlarm().GetHisAlarms(lscUser.LscID, lscUser.LscName, userData.StandardProtocol, lscUser.Group.GroupNodes, beginTime, endTime));
                    var alarms = total.FindAll(a => almIds.ContainsKey(a.AlarmID) && levels.ContainsKey((Int32)a.AlarmLevel));

                    dr1[2] = total.Count;
                    source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[2].ColumnName)] = total;
                    
                    dr1[3] = alarms.Count;
                    source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[3].ColumnName)] = alarms;

                    dr1[4] = String.Format("{0:P2}", total.Count > 0 ? (double)alarms.Count / (double)total.Count : 0);
                    source1.Rows.Add(dr1);
                }
                #endregion
            } else if (countType.Equals("1")) {
                #region Area=2
                foreach (var v in values) {
                    var ids = WebUtility.ItemSplit(v);
                    if (ids.Length != 5) { continue; }

                    var lscId = Int32.Parse(ids[0]);
                    var groupId = Int32.Parse(ids[1]);
                    var nodeId = Int32.Parse(ids[2]);
                    var nodeType = Int32.Parse(ids[3]);
                    var remark = ids[4];
                    var enmNodeType = Enum.IsDefined(typeof(EnmNodeType), nodeType) ? (EnmNodeType)nodeType : EnmNodeType.Null;
                    if (enmNodeType == EnmNodeType.Area && remark.Equals("2")) {
                        condition.Add(new GroupTreeInfo() {
                            LscID = lscId,
                            GroupID = groupId,
                            NodeID = nodeId,
                            NodeType = enmNodeType,
                            Remark = remark
                        });
                    }
                }

                var ls = from c in condition
                         group c by new { c.LscID } into g
                         select new { g.Key.LscID };

                foreach (var l in ls) {
                    var lscUser = userData.LscUsers.Find(lu => { return lu.LscID == l.LscID; });
                    if (lscUser == null) { continue; }

                    var alarms = WebUtility.GetUserAlarms(userData).FindAll(alarm => alarm.LscID == lscUser.LscID && alarm.StartTime >= beginTime && alarm.StartTime <= endTime);
                    alarms.AddRange(new BAlarm().GetHisAlarms(lscUser.LscID, lscUser.LscName, userData.StandardProtocol, lscUser.Group.GroupNodes, beginTime, endTime));

                    var areas = from a in new BOther().GetAreas(lscUser.LscID, lscUser.Group.GroupID, 2)
                                join c in condition on new { a.LscID, a.Area2ID } equals new { c.LscID, Area2ID = c.NodeID }
                                select a;

                    foreach (var area in areas) {
                        var dr1 = source1.NewRow();
                        dr1[1] = area.LscName;
                        dr1[2] = area.Area2Name;

                        var temp1 = alarms.FindAll(a => a.Area2Name.Equals(area.Area2Name, StringComparison.CurrentCultureIgnoreCase));
                        var temp2 = temp1.FindAll(a => almIds.ContainsKey(a.AlarmID) && levels.ContainsKey((Int32)a.AlarmLevel));

                        dr1[3] = temp1.Count;
                        source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[3].ColumnName)] = temp1;

                        dr1[4] = temp2.Count;
                        source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[4].ColumnName)] = temp2;

                        dr1[5] = String.Format("{0:P2}", temp1.Count > 0 ? (double)temp2.Count / (double)temp1.Count : 0);
                        source1.Rows.Add(dr1);
                    }
                }
                #endregion
            } else if (countType.Equals("2")) {
                #region Area=3
                foreach (var v in values) {
                    var ids = WebUtility.ItemSplit(v);
                    if (ids.Length != 5) { continue; }

                    var lscId = Int32.Parse(ids[0]);
                    var groupId = Int32.Parse(ids[1]);
                    var nodeId = Int32.Parse(ids[2]);
                    var nodeType = Int32.Parse(ids[3]);
                    var remark = ids[4];
                    var enmNodeType = Enum.IsDefined(typeof(EnmNodeType), nodeType) ? (EnmNodeType)nodeType : EnmNodeType.Null;
                    if (enmNodeType == EnmNodeType.Area && remark.Equals("3")) {
                        condition.Add(new GroupTreeInfo() {
                            LscID = lscId,
                            GroupID = groupId,
                            NodeID = nodeId,
                            NodeType = enmNodeType,
                            Remark = remark
                        });
                    }
                }

                var ls = from c in condition
                         group c by new { c.LscID } into g
                         select new { g.Key.LscID };

                foreach (var l in ls) {
                    var lscUser = userData.LscUsers.Find(lu => { return lu.LscID == l.LscID; });
                    if (lscUser == null) { continue; }

                    var alarms = WebUtility.GetUserAlarms(userData).FindAll(alarm => alarm.LscID == lscUser.LscID && alarm.StartTime >= beginTime && alarm.StartTime <= endTime);
                    alarms.AddRange(new BAlarm().GetHisAlarms(lscUser.LscID, lscUser.LscName, userData.StandardProtocol, lscUser.Group.GroupNodes, beginTime, endTime));

                    var areas = from a in new BOther().GetAreas(lscUser.LscID, lscUser.Group.GroupID, 3)
                                join c in condition on new { a.LscID, a.Area3ID } equals new { c.LscID, Area3ID = c.NodeID }
                                select a;

                    foreach (var area in areas) {
                        var dr1 = source1.NewRow();
                        dr1[1] = area.LscName;
                        dr1[2] = area.Area2Name;
                        dr1[3] = area.Area3Name;

                        var temp1 = alarms.FindAll(a => a.Area2Name.Equals(area.Area2Name, StringComparison.CurrentCultureIgnoreCase) && a.Area3Name.Equals(area.Area3Name, StringComparison.CurrentCultureIgnoreCase));
                        var temp2 = temp1.FindAll(a => almIds.ContainsKey(a.AlarmID) && levels.ContainsKey((Int32)a.AlarmLevel));

                        dr1[4] = temp1.Count;
                        source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[4].ColumnName)] = temp1;

                        dr1[5] = temp2.Count;
                        source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[5].ColumnName)] = temp2;

                        dr1[6] = String.Format("{0:P2}", temp1.Count > 0 ? (double)temp2.Count / (double)temp1.Count : 0);
                        source1.Rows.Add(dr1);
                    }
                }
                #endregion
            } else if (countType.Equals("3")) {
                #region Station
                foreach (var v in values) {
                    var ids = WebUtility.ItemSplit(v);
                    if (ids.Length != 5) { continue; }

                    var lscId = Int32.Parse(ids[0]);
                    var groupId = Int32.Parse(ids[1]);
                    var nodeId = Int32.Parse(ids[2]);
                    var nodeType = Int32.Parse(ids[3]);
                    var remark = ids[4];
                    var enmNodeType = Enum.IsDefined(typeof(EnmNodeType), nodeType) ? (EnmNodeType)nodeType : EnmNodeType.Null;
                    if (enmNodeType == EnmNodeType.Area && remark.Equals("3")) {
                        condition.Add(new GroupTreeInfo() {
                            LscID = lscId,
                            GroupID = groupId,
                            NodeID = nodeId,
                            NodeType = enmNodeType,
                            Remark = remark
                        });
                    }
                }

                var ls = from c in condition
                         group c by new { c.LscID } into g
                         select new { g.Key.LscID };

                foreach (var l in ls) {
                    var lscUser = userData.LscUsers.Find(lu => { return lu.LscID == l.LscID; });
                    if (lscUser == null) { continue; }

                    var alarms = WebUtility.GetUserAlarms(userData).FindAll(alarm => alarm.LscID == lscUser.LscID && alarm.StartTime >= beginTime && alarm.StartTime <= endTime);
                    alarms.AddRange(new BAlarm().GetHisAlarms(lscUser.LscID, lscUser.LscName, userData.StandardProtocol, lscUser.Group.GroupNodes, beginTime, endTime));

                    var stations = from c in condition
                                   join s in new BOther().GetStations(lscUser.LscID, lscUser.Group.GroupID) on new { c.LscID, Area3ID = c.NodeID } equals new { s.LscID, s.Area3ID }
                                   select s;

                    foreach (var sta in stations) {
                        var dr1 = source1.NewRow();
                        dr1[1] = sta.LscName;
                        dr1[2] = sta.Area2Name;
                        dr1[3] = sta.Area3Name;
                        dr1[4] = sta.StaName;

                        var temp1 = alarms.FindAll(a => a.Area2Name.Equals(sta.Area2Name, StringComparison.CurrentCultureIgnoreCase) && a.Area3Name.Equals(sta.Area3Name, StringComparison.CurrentCultureIgnoreCase) && a.StaName.Equals(sta.StaName, StringComparison.CurrentCultureIgnoreCase));
                        var temp2 = temp1.FindAll(a => almIds.ContainsKey(a.AlarmID) && levels.ContainsKey((Int32)a.AlarmLevel));

                        dr1[5] = temp1.Count;
                        source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[5].ColumnName)] = temp1;

                        dr1[6] = temp2.Count;
                        source2[String.Format("{0}-{1}", source1.Rows.Count, source1.Columns[6].ColumnName)] = temp2;

                        dr1[7] = String.Format("{0:P2}", temp1.Count > 0 ? (double)temp2.Count / (double)temp1.Count : 0);
                        source1.Rows.Add(dr1);
                    }
                }
                #endregion
            }

            var cacheDuration = Int32.Parse(WebConfigurationManager.AppSettings["DefaultCacheDuration"]);
            HttpRuntime.Cache.Insert(cacheKey1, source1, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(cacheDuration), CacheItemPriority.Default, null);
            HttpRuntime.Cache.Insert(cacheKey2, source2, null, Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(cacheDuration), CacheItemPriority.Default, null);
            return source1;
        }

        /// <summary>
        /// Create Customize DataTable
        /// </summary>
        /// <param name="columns">columns</param>
        /// <returns>DataTable</returns>
        private DataTable CreateCustomizeTable(int colLen) {
            var dt = new DataTable();

            //column0
            var col0 = new DataColumn();
            col0.DataType = typeof(String);
            col0.ColumnName = "Data0";
            col0.AutoIncrement = true;
            col0.AutoIncrementSeed = 1;
            col0.AutoIncrementStep = 1;
            dt.Columns.Add(col0);

            //column1-N
            for (int i = 0; i < colLen - 1; i++) {
                var column = new DataColumn();
                column.DataType = typeof(String);
                column.ColumnName = String.Format("Data{0}", i + 1);
                column.DefaultValue = String.Empty;
                dt.Columns.Add(column);
            }
            return dt;
        }

        /// <summary>
        /// Show GridCell Detail
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="rowIndex">rowIndex</param>
        /// <param name="dataIndex">dataIndex</param>
        [DirectMethod(Timeout = 300000)]
        public void ShowGridCellDetail(string title, string rowIndex, string dataIndex) {
            var userData = UserData;
            if (HttpRuntime.Cache[WebUtility.GetCacheKeyName(userData, "major-incident-source2")] == null) {
                AddDataToCache();
            }

            var win = WebUtility.GetNewWindow(800, 600, title, Icon.Printer);
            win.AutoLoad.Url = "~/AlarmWnd.aspx";
            win.AutoLoad.Params.Add(new Ext.Net.Parameter("Type", "major_incident"));
            win.AutoLoad.Params.Add(new Ext.Net.Parameter("Title", title));
            win.AutoLoad.Params.Add(new Ext.Net.Parameter("RowIndex", rowIndex));
            win.AutoLoad.Params.Add(new Ext.Net.Parameter("DataIndex", dataIndex));
            win.Render();
            win.Show();
        }
    }
}