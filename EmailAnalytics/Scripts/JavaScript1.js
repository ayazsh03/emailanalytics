
$("#succuessdialog").dialog({ autoOpen: false });
$("#errordialog").dialog({ autoOpen: false });
$.ajax({
    method: "Get",
    url: "./GetData",
    data : { Date :  "@Model.Item1", SearchString: "@Model.Item2" }
}).done(function (msg) {
    gridShow(msg);
}).error(function (msg) {
    alert(JSON.stringify(msg));
});
var src = [
{ id:"1", value : "Submitted" },
{id :"2", value: "Connected" },
{id : "3", value: "Call" }
];

var ratingsrc = [
    { id:"1", value : "1" },
    {id :"2", value: "2" },
    {id : "3", value: "3" },
    {id : "4", value: "4" },
    {id : "5", value: "5" }
];
function gridShow(data) {

    var source =
    {
        datatype: "json",
        datafields: [
            { name: 'MasterID', type: 'int' },
            { name: 'Rating', type: 'int' },
            { name: 'FirstName', type: 'string' },
            { name: 'LastName', type: 'string' },
            { name: 'Email1', type: 'string' },
            { name: 'PrimaryPhone', type: 'string' },
            { name: 'Url', type: 'string' },
            { name: 'Location', type: 'string' },
            { name: 'CurrentJob', type: 'string' },
            { name: 'Company', type: 'string' },
            { name: 'comment', type: 'string' },
            { name: 'Posted', type: 'date' },
            { name: 'SearchString', type: 'string' },
            { name: 'statusid', type: 'int' },
            { name: 'JobTitle', type: 'string' }
        ],
        localdata: data
    };

    var dataAdapter = new $.jqx.dataAdapter(source);

    var renderer =  function (id, columnfield, value, defaulthtml, columnproperties) {
        var val = "";
        var row=$('#jqxgrid').jqxGrid('getrowdata', id);
        if(1==row.statusid){
            val="Submitted";
        }
        var div = val=="" ? '<img src="~/images/tick_green.png" style="cursor:pointer;margin-left:6px;margin-top:6px;" height="32" width="32" onClick="buttonclick(this.id)" id="btn' + id + '"/>' : defaulthtml;
        return div;
    }


    var Namerenderer =  function (id, columnfield, value, defaulthtml, columnproperties) {
        var val = "";
        var row=$('#jqxgrid').jqxGrid('getrowdata', id);
        var div ='<a target="_blank" href="' + row.Url + '">' + ( '  ' +row.FirstName + ' ' + row.LastName) + '</a>';
        return div;
    }

    var Emailrenderer =  function (id, columnfield, value, defaulthtml, columnproperties) {
        var val = "";
        var row=$('#jqxgrid').jqxGrid('getrowdata', id);
        var div ='<a href="mailto:' + row.Email1 + '">' + row.Email1 + '</a>';
        return div;
    }

    var Actionrenderer = function (id, columnfield, value, defaulthtml, columnproperties) {
        var val = "";
        var row=$('#jqxgrid').jqxGrid('getrowdata', id);
        src.forEach(function(obj) {
            if(obj.id==row.statusid){
                val=obj.value;
            }
        });
        var div = val!="" ? "<span style='margin: 4px; float:" + columnproperties.cellsalign + "; color: #ff0000;'>" + val+ "</span>" : defaulthtml;
        return div;
    }
            
    $("#jqxgrid").jqxGrid(
    {
        width : '100%',
        source: dataAdapter,                
        pageable: true,
        pagesize: 20,
        autoheight: true,
        sortable: true,
        altrows: true,
        enabletooltips: true,
        autorowheight: true,
        autoheight: true,
        editable: true,
        selectionmode: 'multiplecellsadvanced',
        columns: [
            { text: 'Name', align:'center', cellsrenderer:Namerenderer, width:120, editable:false },
            { text: 'Save', align: 'center', cellsrenderer:renderer,width:50, editable:false },
            { text: 'Comment', align: 'center', datafield: 'comment' },
    { text: 'Action', align: 'center',columntype: 'dropdownlist', datafield: 'ActionID',cellsrenderer:Actionrenderer,width:80,
        createeditor: function (row, cellvalue, editor, cellText, width, height) {
            // construct the editor. 
            editor.jqxDropDownList({
                source: src,displayMember: 'value', valueMember: 'id',
                width: width, height: height
            });
        }
    },
            {
                text: 'Rating', width: 50, columntype: 'custom',  datafield: 'Rating',
                createeditor: function (row, cellvalue, editor, cellText, width, height) {
                    editor.jqxDropDownList({
                        source: ratingsrc,displayMember: 'value', valueMember: 'id',
                        width: width, height: height
                    });
                }
            },
            { text: 'Current Job', align: 'center', datafield: 'CurrentJob' , editable:false},
            { text: 'URL', align: 'center', datafield: 'Url', width:100, hidden:true },
            { text: 'Company', align: 'center', datafield: 'Company', editable:false },
            { text: 'Email', align: 'center', datafield: 'Email1', width:'15%', cellsrenderer:Emailrenderer, editable:false },
            { text: 'Phone',align: 'center', datafield: 'PrimaryPhone',width:100 },
            { text: 'Location', align: 'center', datafield: 'Location' ,width:150, editable:false},
            { text: 'First Name', align: 'center', datafield: 'FirstName' , width:'6%', hidden:true },
            { text: 'Last Name', align: 'center', datafield: 'LastName', width:'6%', hidden:true },
            { text: 'MasterID',  datafield: 'MasterID' , hidden:true },
            { text: 'statusid', datafield: 'statusid' , hidden:true },
            { text: 'Posted', align: 'center', datafield: 'Posted',cellsformat: 'd', hidden:true },
            { text: 'SearchString', align: 'center', datafield: 'SearchString', hidden:true },
            { text: 'JobTitle', align: 'center', datafield: 'JobTitle', hidden:true }
        ]
    });

         
}

function buttonclick(event) {
    var rowid = event.replace('btn','');
 @*var getselectedrowindexes = $('#jqxgrid').jqxGrid('getselectedrowindexes');
    if (getselectedrowindexes.length > 0)
    {*@
        // returns the selected row's data.
        var selectedRowData = $('#jqxgrid').jqxGrid('getrowdata', rowid);
    @*}*@

    //alert(JSON.stringify(selectedRowData));
                
    var statusid = 0;
    src.forEach(function(obj) {
        if(obj.value===selectedRowData.ActionID){
            statusid=obj.id;
        }
    });

    //alert(statusid);
             
    var masterData = {
        statusid: statusid,
        Comment:selectedRowData.comment,
        Rating:selectedRowData.Rating,
        MasterID:selectedRowData.MasterID
    };

    $.ajax({
        method: "Get",
        url: "./SaveMasterListReviews",
        data : masterData
    }).done(function (msg) {
        $( "#succuessdialog" ).dialog("open");
        //gridShow(msg);
    }).error(function (msg) {
        $( "#errordialog" ).dialog("open");
    });
                
}

           
       
          
});
}
        
