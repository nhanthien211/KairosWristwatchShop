//Begin Validate Thumbnail File//
var thumbnailFile = document.getElementById('inputFileThumbnail');
thumbnailFile.value = '';
thumbnailFile.onchange = function (e) {
    $('#thumbnailFileUploadResult').removeClass();
    var fileExtension = this.value.match(/\.([^\.]+)$/)[1];
    fileExtension = fileExtension.toLowerCase();
    if (fileExtension === 'png' || fileExtension === 'jpg' || fileExtension === 'jpeg') {
        var size = thumbnailFile.files[0].size;
        if (size > (500 * 1024)) {
            //bigger than 500kb, too much for a thumbnail
            $(this).next().after().text('Choose File');
            $('#thumbnailFileUploadResult').addClass('text-danger');
            $('#thumbnailFileUploadResult').html('Uploaded file must be under 500KB');
            this.value = '';
            document.getElementById('imagePreview').src = '';
            return;
        }
        $(this).next().after().text($(this).val().split('\\').slice(-1)[0]);
        $('#thumbnailFileUploadResult').addClass('text-success');
        $('#thumbnailFileUploadResult').html('Success');
        document.getElementById('imagePreview').src = window.URL.createObjectURL(file.files[0]);
    } else {
        $(this).next().after().text('Choose File');
        $('#thumbnailFileUploadResult').addClass('text-danger');
        $('#thumbnailFileUploadResult').html('Only .jpg .jpeg and .png is supported');
        this.value = '';
        document.getElementById('imagePreview').src = '';
    }

};
//End Validate Thumbnail File//

//Begin Validate Excel File//
var excelFile = document.getElementById('inputFileExcel');
excelFile.value = '';
excelFile.onchange = function (e) {
    $('#excelFileUploadResult').removeClass();
    var fileExtension = this.value.match(/\.([^\.]+)$/)[1];
    fileExtension = fileExtension.toLowerCase();
    if (fileExtension === 'xls' || fileExtension === 'xlsx') {
        var size = excelFile.files[0].size;
        if (size > (5 * 1024 * 1024)) { 
            //bigger than 5MB, too much for an excel file
            $(this).next().after().text('Choose File');
            $('#excelFileUploadResult').addClass('text-danger');
            $('#excelFileUploadResult').html('Uploaded file must be under 5MB');
            this.value = '';            
            return;
        }
        $(this).next().after().text($(this).val().split('\\').slice(-1)[0]);
        $('#excelFileUploadResult').addClass('text-success');
        $('#excelFileUploadResult').html('Success');
        
    } else {
        $(this).next().after().text('Choose File');
        $('#excelFileUploadResult').addClass('text-danger');
        $('#excelFileUploadResult').html('Only .xls and .xlsx is supported');
        this.value = '';        
    }
};
//End Validate Excel File//

//Begin Validate Zip File//
var zipFile = document.getElementById('inputFileZip');
zipFile.value = '';
zipFile.onchange = function (e) {
    $('#zipFileUploadResult').removeClass();
    var fileExtension = this.value.match(/\.([^\.]+)$/)[1];
    fileExtension = fileExtension.toLowerCase();
    if (fileExtension === 'zip') {
        var size = zipFile.files[0].size;
        if (size > (25 * 1024 * 1024)) {
            //bigger than 25MB, too much for a zip file containing 50 thumbnail 500kb
            $(this).next().after().text('Choose File');
            $('#zipFileUploadResult').addClass('text-danger');
            $('#zipFileUploadResult').html('Uploaded file must be under 25MB');
            this.value = '';
            return;
        }
        $(this).next().after().text($(this).val().split('\\').slice(-1)[0]);
        $('#zipFileUploadResult').addClass('text-success');
        $('#zipFileUploadResult').html('Success');

    } else {
        $(this).next().after().text('Choose File');
        $('#zipFileUploadResult').addClass('text-danger');
        $('#zipFileUploadResult').html('Only .zip supported');
        this.value = '';
    }
};
//End Validate Zip File//

function clearFileError() {
    $('#thumbnailFileUploadResult').html('');
    var text = CKEDITOR.instances.editor1.getData();
    text = CKEDITOR.tools.htmlEncode(text);
    $('#hiddenEditorValue').val(text);
}