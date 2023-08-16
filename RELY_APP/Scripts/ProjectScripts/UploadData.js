// $.noConflict();

$(function () {
    
    $("#dialog1").dialog({
        modal: true,
        autoOpen: false,
        title: "FileUpload Dialog Box",
        width: 1000,
        height: 500,
        dialogClass: "custom-dialog"
    });
   
    $("#FileUploadLink").click(function () {
        document.getElementById("IsDataUpload").value = true;
        //$("#dialog").css('zIndex', 9999);
        $('#dialog1').dialog('open');
    });
    
    $('#dialog1').on('dialogclose', function (event) {
        //alert('closed');
        FnUpdateComments1();
    });
    $("#DataUploadDone").click(function () {
        $('#dialog1').dialog('close');
        GetFileList1();
    });
});

function FnUpdateComments1() {
    var Comments = document.getElementById('AttachedComments').value;
    //Make an ajax call to save comments
    $.ajax({
        data: { Comments: Comments },
        cache: false,
        url: "/FileUploader/UpdateComments",
        dataType: "json",
        type: "get",
        success: function (data) {
        },
        error: function (respose) {
        }
    });
}

function GetFileList1() {
    var Comments = document.getElementById('AttachedComments').value;
    //Make an ajax call to get list of files attached in this request
    $.ajax({
        data: { Comments: Comments },
        cache: false,
        url: "/FileUploader/GetUploadedFileList",
        dataType: "json",
        type: "get",
        success: function (data) {
            if (data) {
                var FileList = '<br><ul>';
                //var FileArray=Array.from(data);//this line throws error in IE, therefore using the below line of code.
                var FileArray = data;
                for (var i = 0; i < FileArray.length; i++) {
                    FileList += '<li><div class="col-md-3">' + FileArray[i] + '</div><div class="col-md-2"><a href="#" onclick="RemoveFile(' + i + ')"> <i style="color:red;" class="glyphicon glyphicon-remove"></i></a></div>'
                }
                FileList += '</ul>'
                document.getElementById('FileDiv').innerHTML = FileList;
            }
        },
        error: function (respose) {
            //  alert("error : " + reponse);
        }
    });
}

function RemoveFile1(FileIndex) {
    //Make an ajax call to get list of files attached in this request
    $.ajax({
        data: { FileIndex: FileIndex },
        url: "/FileUploader/DeleteFileFromList",
        dataType: "json",
        cache: false,
        type: "get",
        success: function (data) {
            GetFileList1();
        },
        error: function (respose) {
            //  alert("error : " + reponse);
        }
    });
}

function FnCheckForDuplicateFile1() {
    var files = Array();
    var xx = document.getElementById('file2');
    for (var i = 0; i < xx.files.length; i++) {
        var name = xx.files.item(i).name;
        if (existingFileList != null && existingFileList != 'undefined') {
            if (existingFileList.indexOf(name) > -1) {
                alert("Duplicate file: " + name + " not allowed");
                document.getElementById('file1').value = "";
                return false;
            }
            existingFileList.push(name);
        }
        if (name.indexOf(',') > -1) {
            alert("comma(,) not allowed in name of file.");
            document.getElementById('file1').value = "";
            return false;
        }
        //file type checking
        var IsValid = FnCheckFileTypeValidity(name);
        if (IsValid == false) {//check file type
            document.getElementById('file2').value = "";
            return false;
        }

    }
    return true;
}

function FnCheckFileTypeValidity1(FileName) {
    var ext = FileName.split('.').pop().toLowerCase();
    if ($.inArray(ext, ['jpg', 'png', 'jpeg', 'xls', 'xlsx', 'ppt', 'pptx', 'doc', 'docx', 'pdf', 'txt', 'csv']) == -1) {
        document.getElementById('file2').value = "";
        alert('File with ' + ext + ' not allowed');
        return false;
    }
    else
        return true;
}

var $form = null;
$(function () {

    $form = $('#fileupload1').fileupload({
        dataType: 'json'
    });

});
$('#fileupload1').addClass('fileupload-processing');
$('#fileupload1').bind('fileuploaddone', function (e, data) {

})

