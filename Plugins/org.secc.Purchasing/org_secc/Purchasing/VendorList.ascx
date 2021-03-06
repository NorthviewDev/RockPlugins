﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VendorList.ascx.cs" Inherits="RockWeb.Plugins.org_secc.Purchasing.VendorList" %>
<asp:UpdatePanel ID="updVendorList" runat="server" UpdateMode="Always" class="panel panel-block">
    <ContentTemplate>     

        <div class="panel-heading">
            <h1 class="panel-title"><i class="fa fa-list"></i>&nbsp;Vendor List</h1>
            <asp:placeholder ID="phNewVendor" runat="server">
                <a href="#" ID="lbNewVendor" class="btn-add btn btn-default btn-sm pull-right" OnClick="Rock.controls.modal.show($('this'), '/page/<%= VendorDetailPageSetting.Id %>?t=New Vendor&pb=&sb='); return false;"><i class="fa fa-plus"></i> New Vendor</a>
            </asp:placeholder>
        </div>
        <div class="panel-body">
        <div class="grid grid-panel">
            <Rock:GridFilter runat="server" ID="gfRequisitions" OnApplyFilterClick="btnApplyFilter_Click">
                <Rock:RockTextBox Label="Vendor Name" ID="txtFilterVendorName" runat="server" class="formItem" MaxLength="100" Columns="50" />
                <Rock:RockCheckBox label="Include Inactive" ID="chkIncludeInactive" runat="server" Checked="false" />
            </Rock:GridFilter>
            <Rock:Grid ID="dgVendors" runat="server" AllowSorting="true" AllowPaging="true" DataKeyNames="VendorID" OnRowDataBound="dgVendors_RowDataBound">
                <Columns>
                    <Rock:RockBoundField HeaderText="ID" DataField="VendorID" SortExpression="VendorID" Visible="false" />
                    <Rock:RockTemplateField HeaderText="Name" SortExpression="VendorName">                     
                        <ItemTemplate>
                            <a href="#" onclick="javascript: Rock.controls.modal.show($('this'), '/page/<%=VendorDetailPageSetting.Id %>?t=View Vendor&VendorID=<%# Eval("VendorID") %>&pb=&sb='); return false;"><%# Eval("VendorName") %></a>
                        </ItemTemplate>
                    </Rock:RockTemplateField>

                    <Rock:RockTemplateField HeaderText="Address">                     
                        <ItemTemplate>
                            <%# (Eval("Address") == null) ? "" : ((org.secc.Purchasing.Helpers.Address)Eval("Address")).ToArenaFormat().Replace("^", "&nbsp;") %>
                        </ItemTemplate>
                    </Rock:RockTemplateField>
                    <Rock:RockTemplateField HeaderText="Phone">                     
                        <ItemTemplate>
                            <%# (Eval("Phone") == null) ? "" : ((org.secc.Purchasing.Helpers.PhoneNumber)Eval("Phone")).ToArenaFormat().Replace("^", "&nbsp;") %>
                        </ItemTemplate>
                    </Rock:RockTemplateField> 
                    <asp:HyperLinkField HeaderText="Web" DataNavigateUrlFields="WebAddress" DataTextField="WebAddress" Target="_blank" />
                    <Rock:RockBoundField HeaderText="Terms" DataField="Terms" />
                    <Rock:BoolField HeaderText = "Active" SortExpression="Active" DataField="Active"/>
                    <Rock:RockTemplateField HeaderText="Edit" HeaderStyle-CssClass="grid-columncommand" ItemStyle-CssClass="grid-columncommand">                     
                        <ItemTemplate>
                            <a onclick="javascript: Rock.controls.modal.show($('this'), '/page/<%=VendorDetailPageSetting.Id %>?t=Edit Vendor&VendorID=<%# Eval("VendorID") %>&pb=&sb=&EditMode=true');" class="btn btn-default btn-sm"><i class="fa fa-pencil"></i></a>
                        </ItemTemplate>
                    </Rock:RockTemplateField>
                    <Rock:DeleteField OnClick="Delete_Click" HeaderText="Delete" />
                </Columns>
            </Rock:Grid>
            <Rock:ModalAlert ID="maAlert" runat="server" />
        </div>
        <script type="text/javascript">
            $(document).ready(function ()
            {
                el = $('.grid-filter header').get();
                $('i.toggle-filter', el).toggleClass('fa-chevron-down fa-chevron-up');
                $(el).siblings('div').slideDown(0);
            });
        </script>
        <style>
            /*.grid-filter h4 {
                display: none;
            }
            .grid-filter header {
                display: none;
            }*/
            td.disabled a {
                background-color: #e3baba !important;
                border-color: #d7a2a2 !important;
            }
        </style>
    </ContentTemplate>
</asp:UpdatePanel>