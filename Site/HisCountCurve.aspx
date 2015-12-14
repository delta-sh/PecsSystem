﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HisCountCurve.aspx.cs" Inherits="Delta.PECS.WebCSC.Site.HisCountCurve" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>历史统计曲线</title>
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder1" runat="server" Mode="Script" />
    <ext:ResourcePlaceHolder ID="ResourcePlaceHolder2" runat="server" Mode="Style" />
    <link rel="shortcut icon" type="image/x-icon" href="favicon.ico" />
    <link rel="icon" type="image/x-icon" href="favicon.ico" />
    <link rel="bookmark" type="image/x-icon" href="favicon.ico" />
    <script type="text/javascript" src="Resources/amcharts/amcharts.js?v=3.3.0.0"></script>
    <script type="text/javascript" src="Resources/js/amcharts.js?v=3.3.0.0"></script>
    <script type="text/javascript">
        //<![CDATA[
        AmCharts.ready(function() {
            HisCntCurve.render();
        });
        //]]>
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" DirectMethodNamespace="X" IDMode="Explicit" />
    <ext:Viewport ID="HisCountCurveViewport" runat="server" Layout="BorderLayout">
        <Items>
            <ext:Panel ID="ChartPanel" runat="server" AutoScroll="true" Region="Center" Title="历史统计曲线" Icon="ChartCurve">
                <TopBar>
                    <ext:Toolbar ID="HisCountCurveConditionBar" runat="server" Layout="ContainerLayout">
                        <Items>
                            <ext:Toolbar ID="HisCountCurveConditionBar1" runat="server" Flat="true">
                                <Items>
                                    <ext:ComboBox ID="LscsComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="Lsc名称"
                                        Editable="true" DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local"
                                        ForceSelection="true" TriggerAction="All" SelectOnFocus="true" Resizable="true">
                                        <Store>
                                            <ext:Store ID="LscsStore" runat="server" AutoLoad="true" OnRefreshData="OnLscsRefresh">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Name" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <Listeners>
                                                    <Load Handler="
                                                    var sctId = #{LscHF}.getValue();
                                                    #{LscHF}.setValue('');
                                                    if(this.getCount() != 0){
                                                        if(Ext.isEmpty(sctId, false) 
                                                        || this.findExact('Id',sctId) == -1){
                                                            sctId = this.getAt(0).get('Id');
                                                        }
                                                        #{LscsComboBox}.setValueAndFireSelect(sctId);
                                                    }" />
                                                </Listeners>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="#{Area2ComboBox}.clearValue();#{Area2ComboBox}.store.reload();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="Area2ComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="地区名称"
                                        Editable="true" DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local"
                                        ForceSelection="true" TriggerAction="All" SelectOnFocus="true" Resizable="true">
                                        <Store>
                                            <ext:Store ID="Area2Store" runat="server" AutoLoad="false" OnRefreshData="OnArea2Refresh">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Name" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <Listeners>
                                                    <Load Handler="
                                                    var sctId = #{Area2HF}.getValue();
                                                    #{Area2HF}.setValue('');
                                                    if(this.getCount() != 0) {
                                                        if(Ext.isEmpty(sctId, false)
                                                        || this.findExact('Id',sctId) == -1){
                                                            sctId = this.getAt(0).get('Id');
                                                        }
                                                        #{Area2ComboBox}.setValueAndFireSelect(sctId);
                                                    } else {
                                                        #{Area2ComboBox}.fireEvent('select');
                                                    }" />
                                                </Listeners>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="#{Area3ComboBox}.clearValue(); #{Area3ComboBox}.store.reload();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="Area3ComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="县市名称"
                                        Editable="true" DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local"
                                        ForceSelection="true" TriggerAction="All" SelectOnFocus="true" Resizable="true">
                                        <Store>
                                            <ext:Store ID="Area3Store" runat="server" AutoLoad="false" OnRefreshData="OnArea3Refresh">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Name" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <Listeners>
                                                    <Load Handler="
                                                    var sctId = #{Area3HF}.getValue();
                                                    #{Area3HF}.setValue('');
                                                    if(this.getCount() != 0) {
                                                        if(Ext.isEmpty(sctId, false)
                                                        || this.findExact('Id',sctId) == -1){
                                                            sctId = this.getAt(0).get('Id');
                                                        }
                                                        #{Area3ComboBox}.setValueAndFireSelect(sctId);
                                                    } else {
                                                        #{Area3ComboBox}.fireEvent('select');
                                                    }" />
                                                </Listeners>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="#{StaComboBox}.clearValue(); #{StaComboBox}.store.reload();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="StaComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="局站名称"
                                        Editable="true" DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local"
                                        ForceSelection="true" TriggerAction="All" SelectOnFocus="true" Resizable="true">
                                        <Store>
                                            <ext:Store ID="StaStore" runat="server" AutoLoad="false" OnRefreshData="OnStaRefresh">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Name" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <Listeners>
                                                    <Load Handler="
                                                    var sctId = #{StaHF}.getValue();
                                                    #{StaHF}.setValue('');
                                                    if(this.getCount() != 0) {
                                                        if(Ext.isEmpty(sctId, false)
                                                        || this.findExact('Id',sctId) == -1){
                                                            sctId = this.getAt(0).get('Id');
                                                        }
                                                        #{StaComboBox}.setValueAndFireSelect(sctId);
                                                    }" />
                                                </Listeners>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="#{DevComboBox}.clearValue(); #{DevComboBox}.store.reload();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                    <ext:ComboBox ID="DevComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="设备名称"
                                        Editable="true" DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local"
                                        ForceSelection="true" TriggerAction="All" SelectOnFocus="true" Resizable="true">
                                        <Store>
                                            <ext:Store ID="DevStore" runat="server" AutoLoad="false" OnRefreshData="OnDevRefresh">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Name" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <Listeners>
                                                    <Load Handler="
                                                    var sctId = #{DevHF}.getValue();
                                                    #{DevHF}.setValue('');
                                                    if(this.getCount() != 0) {
                                                        if(Ext.isEmpty(sctId, false)
                                                        || this.findExact('Id',sctId) == -1) {
                                                            sctId = this.getAt(0).get('Id');
                                                        }
                                                        #{DevComboBox}.setValueAndFireSelect(sctId);
                                                    }" />
                                                </Listeners>
                                            </ext:Store>
                                        </Store>
                                        <Listeners>
                                            <Select Handler="#{NodeComboBox}.clearValue(); #{NodeComboBox}.store.reload();" />
                                        </Listeners>
                                    </ext:ComboBox>
                                </Items>
                            </ext:Toolbar>
                            <ext:Toolbar ID="HisCountCurveConditionBar2" runat="server" Flat="true">
                                <Items>
                                    <ext:ComboBox ID="NodeComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="测点名称"
                                        Editable="true" DisplayField="Name" ValueField="Id" TypeAhead="true" Mode="Local"
                                        ForceSelection="true" TriggerAction="All" SelectOnFocus="true" Resizable="true">
                                        <Store>
                                            <ext:Store ID="NodeStore" runat="server" AutoLoad="false" OnRefreshData="OnNodeRefresh">
                                                <Proxy>
                                                    <ext:PageProxy />
                                                </Proxy>
                                                <Reader>
                                                    <ext:JsonReader IDProperty="Id">
                                                        <Fields>
                                                            <ext:RecordField Name="Id" Type="String" />
                                                            <ext:RecordField Name="Name" Type="String" />
                                                        </Fields>
                                                    </ext:JsonReader>
                                                </Reader>
                                                <Listeners>
                                                    <Load Handler="
                                                    var sctId = #{NodeHF}.getValue();
                                                    #{NodeHF}.setValue('');
                                                    if(this.getCount() != 0) {
                                                        if(Ext.isEmpty(sctId, false) 
                                                        || this.findExact('Id',sctId) == -1) {
                                                            sctId = this.getAt(0).get('Id');
                                                        }
                                                        #{NodeComboBox}.setValueAndFireSelect(sctId);
                                                    }" />
                                                </Listeners>
                                            </ext:Store>
                                        </Store>
                                    </ext:ComboBox>
                                    <ext:TriggerField ID="FromDate" runat="server" Width="180" LabelWidth="50" FieldLabel="开始时间">
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Date" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="WdatePicker({el:'FromDate',isShowClear:false,readOnly:true,isShowWeek:true,maxDate:'%y-%M-%d %H:%m:%s',onpicked:function(){#{ToDate}.fireEvent('triggerclick')}});" />
                                        </Listeners>
                                    </ext:TriggerField>
                                    <ext:TriggerField ID="ToDate" runat="server" Width="180" LabelWidth="50" FieldLabel="结束时间">
                                        <Triggers>
                                            <ext:FieldTrigger Icon="Date" />
                                        </Triggers>
                                        <Listeners>
                                            <TriggerClick Handler="WdatePicker({el:'ToDate',isShowClear:false,readOnly:true,isShowWeek:true,minDate:'#F{$dp.$D(\'FromDate\')}',maxDate:'%y-%M-%d %H:%m:%s'});" />
                                        </Listeners>
                                    </ext:TriggerField>
                                    <ext:ComboBox ID="QueryTypeComboBox" runat="server" Width="180" LabelWidth="50" FieldLabel="查询方式"
                                        ForceSelection="true" SelectOnFocus="true" Resizable="true">
                                        <Items>
                                            <ext:ListItem Text="日曲线" Value="0" />
                                            <ext:ListItem Text="月曲线" Value="1" />
                                            <ext:ListItem Text="年曲线" Value="2" />
                                        </Items>
                                        <SelectedItem Value="0" />
                                    </ext:ComboBox>
                                    <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="5" />
                                    <ext:SplitButton ID="QueryBtn" runat="server" Text="查询" Icon="Magnifier" StandOut="true">
                                        <Menu>
                                            <ext:Menu ID="QueryMenu1" runat="server">
                                                <Items>
                                                    <ext:MenuItem ID="SaveMenuItem" runat="server" Text="打印/导出" Icon="Printer">
                                                        <DirectEvents>
                                                            <Click OnEvent="SaveBtn_Click" IsUpload="true">
                                                                <ExtraParams>
                                                                    <ext:Parameter Name="ColumnNames" Mode="Raw" Value="HisCntCurve.getColumnNames(#{HisCountCurvePanel})" />
                                                                </ExtraParams>
                                                            </Click>
                                                        </DirectEvents>
                                                    </ext:MenuItem>
                                                </Items>
                                            </ext:Menu>
                                        </Menu>
                                        <DirectEvents>
                                            <Click OnEvent="QueryBtn_Click" Success="#{HisCountCurvePagingToolbar}.doLoad();HisCntCurve.setData();">
                                                <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{HisCountCurvePanel}.body.up('div')" />
                                            </Click>
                                        </DirectEvents>
                                    </ext:SplitButton>
                                </Items>
                            </ext:Toolbar>
                        </Items>
                    </ext:Toolbar>
                </TopBar>
                <Content>
                    <div id="chartContainer">
                    </div>
                </Content>
                <Listeners>
                    <BeforeRender Fn="HisCntCurve.setCenterPanel" Scope="HisCntCurve" />
                </Listeners>
            </ext:Panel>
            <ext:GridPanel ID="HisCountCurvePanel" runat="server" StripeRows="true" Header="false"
                TrackMouseOver="true" AutoScroll="true" Split="true" Collapsible="true" CollapseMode="Mini"
                Border="false" Region="South">
                <Store>
                    <ext:Store ID="HisCountCurveStore" runat="server" AutoLoad="false" OnRefreshData="OnHisCountCurveListRefresh">
                        <Proxy>
                            <ext:PageProxy />
                        </Proxy>
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="ID" Type="Int" />
                                    <ext:RecordField Name="LscID" Type="Int" />
                                    <ext:RecordField Name="AvgValue" Type="Float" />
                                    <ext:RecordField Name="MaxValue" Type="Float" />
                                    <ext:RecordField Name="MinValue" Type="Float" />
                                    <ext:RecordField Name="MaxTime" Type="String" />
                                    <ext:RecordField Name="MinTime" Type="String" />
                                    <ext:RecordField Name="StartTime" Type="String" />
                                    <ext:RecordField Name="EndTime" Type="String" />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <BaseParams>
                            <ext:Parameter Name="start" Value="0" Mode="Raw" />
                            <ext:Parameter Name="limit" Value="#{PageSizeComboBox}.getValue()" Mode="Raw" />
                        </BaseParams>
                    </ext:Store>
                </Store>
                <LoadMask ShowMask="true" />
                <ColumnModel ID="HisCountCurveColumnModel" runat="server">
                    <Columns>
                        <ext:Column Header="序号" DataIndex="ID" Align="Left" />
                        <ext:Column Header="平均值" DataIndex="AvgValue" Align="Left" />
                        <ext:Column Header="最大值" DataIndex="MaxValue" Align="Left" />
                        <ext:Column Header="最大值时间" DataIndex="MaxTime" Align="Center" Width="150" />
                        <ext:Column Header="最小值" DataIndex="MinValue" Align="Left" />
                        <ext:Column Header="最小值时间" DataIndex="MinTime" Align="Center" Width="150" />
                        <ext:Column Header="开始时间" DataIndex="StartTime" Align="Center" Width="150" />
                        <ext:Column Header="结束时间" DataIndex="EndTime" Align="Center" Width="150" />
                    </Columns>
                </ColumnModel>
                <SelectionModel>
                    <ext:RowSelectionModel ID="HisCountCurveRowSelectionModel" runat="server" SingleSelect="true" />
                </SelectionModel>
                <View>
                    <ext:GridView ID="HisCountCurveGridView" runat="server" ForceFit="false" />
                </View>
                <Listeners>
                    <BeforeRender Fn="HisCntCurve.setSouthPanel" Scope="HisCntCurve" />
                </Listeners>
                <BottomBar>
                    <ext:PagingToolbar ID="HisCountCurvePagingToolbar" runat="server" PageSize="20" StoreID="HisCountCurveStore">
                        <Items>
                            <ext:ComboBox ID="PageSizeComboBox" runat="server" Width="150" LabelWidth="60" FieldLabel="<%$ Resources: GlobalResource, ComboBoxPageSize %>">
                                <Items>
                                    <ext:ListItem Text="10" Value="10" />
                                    <ext:ListItem Text="20" Value="20" />
                                    <ext:ListItem Text="50" Value="50" />
                                    <ext:ListItem Text="100" Value="100" />
                                    <ext:ListItem Text="200" Value="200" />
                                    <ext:ListItem Text="500" Value="500" />
                                </Items>
                                <SelectedItem Value="20" />
                                <Listeners>
                                    <Select Handler="#{HisCountCurvePagingToolbar}.pageSize = parseInt(this.getValue()); #{HisCountCurvePagingToolbar}.doLoad();" />
                                </Listeners>
                            </ext:ComboBox>
                        </Items>
                    </ext:PagingToolbar>
                </BottomBar>
            </ext:GridPanel>
        </Items>
    </ext:Viewport>
    <ext:Hidden ID="LscHF" runat="server" />
    <ext:Hidden ID="Area2HF" runat="server" />
    <ext:Hidden ID="Area3HF" runat="server" />
    <ext:Hidden ID="StaHF" runat="server" />
    <ext:Hidden ID="DevHF" runat="server" />
    <ext:Hidden ID="NodeHF" runat="server" />
    </form>
</body>
</html>
