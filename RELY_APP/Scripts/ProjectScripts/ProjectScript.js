
//This function will call the server method to fill dropdown 
function FnFillDropDown(DropdownId, ColumnName, TransactionId, FormType, DefaultValue)
{

    $.ajax({
        data: { DropdownId: DropdownId, TransactionId: TransactionId, ColumnName: ColumnName, FormType: FormType, DefaultValue: DefaultValue},
        url: "/LLocalPOB/GetDropDownValue",
        dataType: "json",
        type: "POST",
        success: function (response) {
            var data  = response.model;
            var markup = '<option value="">-- Select Here --</option>';
            var DropDownName = response.DropdownName;
            //if (DropDownName == 'ContractDuration') { //when Column is ContractDuration, dropdown needs to display Description instead of Value
            //    for (var x = 0; x < data.length; x++) {
            //        if (data[x].Value == data[x].SelectedValue) {
            //            markup += '<option selected="selected" value="' + data[x].Value + '">' + data[x].Description + '</option>';
            //        }
            //        else {
            //            markup += '<option value="' + data[x].Value + '">' + data[x].Description + '</option>';
            //        }
            //    }
            //} else {
                for (var x = 0; x < data.length; x++) {
                    if (data[x].Value == data[x].SelectedValue) {
                        markup += '<option selected="selected" value="' + data[x].Value + '">' + data[x].Description + '</option>';
                    }
                    else {
                        markup += '<option value="' + data[x].Value + '">' + data[x].Description + '</option>';
                    }
                }
           // }
            $("#" + ColumnName).html(markup).show();

        },
        error: function (reponse) {
            //  alert("error : " + reponse);
        }
    });
}

function GenerateCompanySpecificForm(CompanySpecificArray,FormType,TransactionId)
{

    //Order the form field as per ordinal number of Companyspecificdata
   // console.log(CompanySpecificArray);
    /*Loop through company specific data and add divs on screen as per ordinal number of the field*/
    var ElementToBeAdded = '';
    var body = document.getElementById('DIVAttributeContainer');
    //As ProductPob is contained within Product form, both of them contain attribute columns and need to populate in separate div.
    if(String(FormType) == 'LProductPobs'){
            body = document.getElementById('DIVProductPOBAttributeContainer');
    }
    for (var i = 0; i < CompanySpecificArray.length; ++i) {
        var DefaultValue = CompanySpecificArray[i].DefaultValue;
        
    //check on the basis of datatype
    //creating DropDown/Textbox for string type attributes
    if (String(CompanySpecificArray[i].DataType) == 'nvarchar')
    {
        if(DefaultValue == null){
             DefaultValue = '';
        }
            //create parent div
           var newdiv = document.createElement('div');   //create a div
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            body.insertBefore(newdiv,body.lastChild);

            // Creating label
            var newlabel = document.createElement("Label");
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName; 
                newlabel.setAttribute("for",String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for",String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);
        
            var subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);

            //if dropdown Id is available, then create DropDown Box,else create Editor
            if(CompanySpecificArray[i].DropDownId){
                 var SelectElement = document.createElement("select");
                if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    SelectElement.id =  String(FormType) + CompanySpecificArray[i].ColumnName; 
                    SelectElement.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
                } else {
                    SelectElement.id =  CompanySpecificArray[i].ColumnName;
                    SelectElement.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
               // SelectElement.id = CompanySpecificArray[i].ColumnName;
                
                SelectElement.setAttribute("class", "form-control input-sm");
                SelectElement.setAttribute("maxlength",CompanySpecificArray[i].MaximumLength);  
               // SelectElement.setAttribute("value",DefaultValue); 
                SelectElement.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(SelectElement);
            } 
            else if(CompanySpecificArray[i].IsMultiline)
            {
                var element = document.createElement("input");
                element.setAttribute("type", "textarea");
                if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    element.id =  String(FormType) + CompanySpecificArray[i].ColumnName; 
                    element.setAttribute("name",String(FormType) + CompanySpecificArray[i].ColumnName);
                   
                } else {
                    element.id =   CompanySpecificArray[i].ColumnName;
                     element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
               // element.id = "Lbl" + CompanySpecificArray[i].ColumnName;
               // element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                element.setAttribute("maxlength",CompanySpecificArray[i].MaximumLength);  
                //element.setAttribute("value",DefaultValue); 
                element.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(element);
                element.setAttribute("class", "form-control");
                element.setAttribute("class", "textarea1");
                element.setAttribute("style", "min-height:50px;");
                
            }
            else
            {
                var element = document.createElement("input");
                element.setAttribute("type", "text");
                if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    element.id = String(FormType) + CompanySpecificArray[i].ColumnName; 
                    element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
                } else {
                    element.id =  CompanySpecificArray[i].ColumnName;
                    element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
                element.setAttribute("class", "form-control input-sm");
                element.setAttribute("maxlength",CompanySpecificArray[i].MaximumLength);
               // element.setAttribute("value",DefaultValue);
                element.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(element);
            }
//Validation Message display by below code
var  Validateelement = document.createElement("span");
            Validateelement.setAttribute("class", "field-validation-valid text-danger");
            if(String(FormType) == 'LProductPobs'){ 
                 Validateelement.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
             } else {
                 Validateelement.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

             }
           Validateelement.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(Validateelement);
        
        }
//create input type number for numeric types
    if ((String(CompanySpecificArray[i].DataType) == 'numeric'))
    {
        
            //create parent div
           var newdiv = document.createElement('div');   //create a div
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            body.insertBefore(newdiv,body.lastChild);

            // Creating label
            var newlabel = document.createElement("Label");
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName; 
                newlabel.setAttribute("for",String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for",String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);
        
            var subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);

            
                var element = document.createElement("input");
                element.setAttribute("type", "number");
                if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    element.id = String(FormType) + CompanySpecificArray[i].ColumnName; 
                    element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
                } else {
                    element.id =  CompanySpecificArray[i].ColumnName;
                    element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
                element.setAttribute("class", "form-control input-sm");
               // element.setAttribute("value",DefaultValue);
                element.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(element);
        //Validation Message display by below code
        element = document.createElement("span");
            element.setAttribute("class", "field-validation-valid text-danger");
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names 
                 element.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
             } else {
                 element.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

             }
           element.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(element);
    }

        //create input type number for int types
        if ((String(CompanySpecificArray[i].DataType) == 'int')) {
            //create parent div
            var newdiv = document.createElement('div');   //create a div
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            body.insertBefore(newdiv, body.lastChild);

            // Creating label
            var newlabel = document.createElement("Label");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);

            var subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);


            var element = document.createElement("input");
            element.setAttribute("type", "number");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                element.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.id = CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", CompanySpecificArray[i].ColumnName);
            }
            element.setAttribute("class", "form-control input-sm");
            // element.setAttribute("value",DefaultValue);
            element.innerHTML = CompanySpecificArray[i].ColumnName;
            var elementId = CompanySpecificArray[i].ColumnName;
            element.onblur = function (elementId) {
                var sourceElement = elementId.currentTarget.id;
                var val = document.getElementById("" + sourceElement).value;
                var countDot = (val.match(/\./g) || []).length;
                if (countDot > 0) {
                  //  alert("Decimal Point not allowed.");
                    document.getElementById("" + sourceElement).value = '';
                    return;
                }
            };
            subDiv.appendChild(element);
            //Validation Message display by below code
            element = document.createElement("span");
            element.setAttribute("class", "field-validation-valid text-danger");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names 
                element.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

            }
            element.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(element);
        }
        //create checkboxes for Bit type attributes
        if (String(CompanySpecificArray[i].DataType) == 'bit')
        {
             newdiv = document.createElement('div');   //create a div for CheckBox
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("style", "min-height:80px;");
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            body.appendChild(newdiv);
            //Create Label for CheckBox
            newlabel = document.createElement("Label");
           
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                 newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName; 
                 newlabel.setAttribute("for",String(FormType) + String(CompanySpecificArray[i].ColumnName));
             } else {
                 newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                 newlabel.setAttribute("for",String(CompanySpecificArray[i].ColumnName));
             }
            //newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
            newlabel.setAttribute("class", "col-md-5 col-md-5 rely-labels");
            //newlabel.setAttribute("style", "text-align: right;");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);

            subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-7 col-lg-7");
            newdiv.appendChild(subDiv);
            //create checkbox
            element = document.createElement("input");
            element.setAttribute("type", "checkbox");
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                 element.id =  String(FormType) + CompanySpecificArray[i].ColumnName;  
                 element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
             } else {
                 element.id =  CompanySpecificArray[i].ColumnName;
                 element.setAttribute("name", CompanySpecificArray[i].ColumnName);

             }
            //element.id = "Lbl" + CompanySpecificArray[i].ColumnName;
           
           // element.setAttribute("class", "form-control");
            element.innerHTML = CompanySpecificArray[i].ColumnName;
            var elementId = CompanySpecificArray[i].ColumnName;
            element.onclick = function (elementId) {
                var sourceElement = elementId.currentTarget.id;
                var val = document.getElementById("" + sourceElement + "").checked ? true : false;
                CheckBoxAttributeValues = FnReplaceAttributeValues(CheckBoxAttributeValues, sourceElement, val);
            };

            subDiv.appendChild(element);
            //Validation Message display by below code
             element = document.createElement("span");
            element.setAttribute("class", "field-validation-valid text-danger");
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names 
                 element.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
             } else {
                 element.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

             }
           element.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(element);
          // subDiv.appendChild'<span class="field-validation-valid text-danger" data-valmsg-for="'+element.id+'" data-valmsg-replace="true"> </span>'
        }
        //create DateTime for Date type attributes
        if (String(CompanySpecificArray[i].DataType) == 'Datetime' || String(CompanySpecificArray[i].DataType) == 'datetime' || String(CompanySpecificArray[i].DataType) == 'date' ){
        //if (String(CompanySpecificArray[i].ColumnName) == 'AttributeD01' || String(CompanySpecificArray[i].ColumnName) == 'AttributeD02' || String(CompanySpecificArray[i].ColumnName) == 'AttributeD03' || String(CompanySpecificArray[i].ColumnName) == 'AttributeD04' || String(CompanySpecificArray[i].ColumnName) == 'AttributeD05') {

             newdiv = document.createElement('div');   //create a div for CheckBox
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            //Create Label for CheckBox
            newlabel = document.createElement("Label");
              if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                 newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName; 
                 newlabel.setAttribute("for",String(FormType) + String(CompanySpecificArray[i].ColumnName));
             } else {
                 newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                 newlabel.setAttribute("for",String(CompanySpecificArray[i].ColumnName));
             }
           
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");//SS changed this line
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);
            subDiv = document.createElement('div');
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);

            subDiv1 = document.createElement('div');   //create a sub div
            if(String(FormType) == 'LProductPobs'){
                subDiv1.id = String(FormType) + CompanySpecificArray[i].ColumnName;
            } else {
                subDiv1.id = CompanySpecificArray[i].ColumnName;
            }
            //subDiv.setAttribute("class", "col-md-11 col-lg-11");

            subDiv.appendChild(subDiv1);

            //need to create Datetime picker here
            if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                 $("#" + String(FormType) + CompanySpecificArray[i].ColumnName).jqxDateTimeInput({width:'200px',height: '30px', placeHolder: "DD/MM/YYYY", value: null });
             } else {
                 $("#" + CompanySpecificArray[i].ColumnName).jqxDateTimeInput({width:'200px',height: '30px', placeHolder: "DD/MM/YYYY", value: null });
             }
        }

        //Setting Default Value in the fields in case of create
       
        if (TransactionId == -1)
        {
            
            if (String(CompanySpecificArray[i].DataType) == "bit")
            {
                if (DefaultValue != null && DefaultValue.toLocaleLowerCase() == "true")
                   {
                    document.getElementById(String(CompanySpecificArray[i].ColumnName)).checked = true;
                     }

                else if (DefaultValue != null && DefaultValue.toLocaleLowerCase() == "false")
                {
                    document.getElementById(String(CompanySpecificArray[i].ColumnName)).checked = false;
                }
            }
            else if (String(CompanySpecificArray[i].DataType) == "datetime" || String(CompanySpecificArray[i].DataType) == "date")
            {
                var ArrdateTime = [];
                if (DefaultValue != null && DefaultValue != "")
                {
                    ArrdateTime = DefaultValue.split("/");
                    var year = ArrdateTime[0];
                    var Month = ArrdateTime[1];
                    var day = ArrdateTime[2];
                    var Connection = new Date(year, Month - 1, day);
                    $('#' + CompanySpecificArray[i].ColumnName).jqxDateTimeInput('setDate', Connection);
                }
            }
            else
            {
                document.getElementById(String(CompanySpecificArray[i].ColumnName)).value = DefaultValue;
            }
        }

        //Make a server call to get dropdown values to be displayed if dropdownId is present in that column
        if (CompanySpecificArray[i].DropDownId) {
            var DropdownId = Number(CompanySpecificArray[i].DropDownId);
            var ColumnName = CompanySpecificArray[i].ColumnName;
            FnFillDropDown(DropdownId, ColumnName, TransactionId, FormType, DefaultValue);
            
             }
        //Add tooltip which is same as text box value as directed by JS
        if (document.getElementById(String(CompanySpecificArray[i].ColumnName))) {

            
            var TooltipValue = "";
            if ($('#'+String(CompanySpecificArray[i].ColumnName)).is("select")) {
                // the input field is  a dropdown
                TooltipValue = $("#" + String(CompanySpecificArray[i].ColumnName)+" option:selected").text();
            }
            else {
                TooltipValue = document.getElementById(String(CompanySpecificArray[i].ColumnName)).value;
            }
            //document.getElementById(String(CompanySpecificArray[i].ColumnName)).title = TooltipValue;
        }

        //check if IsManadatory is true then dynamically add validation to those fields
        if (Boolean(CompanySpecificArray[i].IsMandatory) == true) {

             if (String(CompanySpecificArray[i].DataType) == 'nvarchar'){
                if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    if (document.getElementById(String(FormType) + CompanySpecificArray[i].ColumnName)) {
                        $('#' + String(FormType) + String(CompanySpecificArray[i].ColumnName)).prop('required', true);
                     }
                } else {
                   if (document.getElementById(CompanySpecificArray[i].ColumnName)) {
                        $('#' + String(CompanySpecificArray[i].ColumnName)).prop('required', true);
                    }
                }                       
                
             }
            //Add red star on required columns
            //$("#Lbl" + CompanySpecificArray[i].ColumnName).addClass("required");
                if(String(FormType) == 'LProductPobs'){ //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                        $("#Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName).addClass("required");

                } else {
                        $("#Lbl" + CompanySpecificArray[i].ColumnName).addClass("required");
                }
        }

        //replace custom label with Default Label
         if(String(FormType) == 'LProductPobs'){
            if (CompanySpecificArray[i].Label && document.getElementById("Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName)) {
                document.getElementById("Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName).innerHTML = CompanySpecificArray[i].Label;
            }
         }
         else{        
            if (CompanySpecificArray[i].Label && document.getElementById("Lbl" + CompanySpecificArray[i].ColumnName)) {
                document.getElementById("Lbl" + CompanySpecificArray[i].ColumnName).innerHTML = CompanySpecificArray[i].Label;
            }
        }


        //Where(p => p.ColumnName == "PayeeId").Where(p => p.DisplayOnForm == true)
        if (Boolean(CompanySpecificArray[i].DisplayOnForm) == true) {
            if (document.getElementById('DIV' + CompanySpecificArray[i].ColumnName)) {
                ElementToBeAdded += document.getElementById('DIV' + CompanySpecificArray[i].ColumnName).outerHTML;
            }
        }

    }
    /*Now remove the UnOrdered Div from the screen*/
    $('#UnOrderedForm').empty();
    /*Add Ordered Div on the Form*/
    $('#OrderedForm').html(ElementToBeAdded);

}




//function FnClickSecondaryButtons(ActionName, WorkflowId, StepId, Source, TransactionId, GlobalComment, StepParticipantActionId,ActionUrl)
//{
//    //replace \n characters in comments with @ as \n is being dropped on reaching to controller
//    var Comments = GlobalComment.split('\n').join('@\n');
//    GlobalComment = Comments;
//    //code generalized
//    var UrlString = "";
//    if (ActionUrl.indexOf("UpdateActionStatus") > -1 || ActionUrl.indexOf("ResetPassword") > -1) {
//        //UrlString = ActionUrl + "/" + TransactionId + "?ActionName=" + ActionName + "&TransactionId=" + TransactionId + "&Comments=" + GlobalComment + "&WorkflowId=" + WorkflowId + "&StepId=" + StepId + "&Source=" + Source + "&StepParticipantActionId=" + StepParticipantActionId;
//        UrlString = ActionUrl + "?TransactionId=" + TransactionId + "&Comments=" + GlobalComment + "&Source=" + Source + "&StepParticipantActionId=" + StepParticipantActionId;
//    }
//    else {
//        UrlString = ActionUrl + "/" + TransactionId + "?FormType=" + ActionName + "&Source=" + Source;
//    }
//    window.location.href = UrlString;
//    /*
//    switch (ActionName) {
//        case "Download":
//            window.location.href = '/GenericGrid/Download?TransactionId=' + TransactionId;
//            break;

//        case "Edit":
//            window.location.href = '/GenericGrid/Edit?ActionName=' + ActionName + '&TransactionId=' + TransactionId + '&Comments=' + GlobalComment + '&WorkflowId=' + WorkflowId + '&StepId=' + StepId + '&Source=' + Source + '&StepParticipantActionId=' + StepParticipantActionId;
//            break;
//        case "Review":
//            window.location.href = '/GenericGrid/Review?TransactionId=' + TransactionId + '&WorkflowId=' + WorkflowId + '&StepId=' + StepId + '&StepParticipantActionId=' + StepParticipantActionId;
//            break;
//        case "Cancel":
//            if (confirm('Once cancelled this item will be permanently removed from workflow. Are you sure you want to proceed?')) {
//               // window.location.href = '/GenericGrid/UpdateActionStatus?ActionName=' + ActionName + '&TransactionId=' + TransactionId + '&Comments=' + GlobalComment + '&WorkflowId=' + WorkflowId + '&StepId=' + StepId + '&StepParticipantActionId=' + StepParticipantActionId;
//            }
//            else {
//                return;
//            }
//            //break;
//        case "Approve":
//        case "Suspend":
//        case "Withdraw":
//        case "Previous":
//        case "SelfAssign":
//        case "SendToRequester":
//        case "Resume":
//        case "SetCompleted":
//        case "Duplicate":
//        case "SendToStep"://new action added
//                window.location.href = '/GenericGrid/UpdateActionStatus?ActionName=' + ActionName + '&TransactionId=' + TransactionId + '&Comments=' + GlobalComment + '&WorkflowId=' + WorkflowId + '&StepId=' + StepId + '&StepParticipantActionId=' + StepParticipantActionId;
//                break;
//        case "ResetPassword":
//            window.location.href = '/Account/ResetPasswordViaAdmin?ActionName=' + ActionName + '&TransactionId=' + TransactionId + '&Comments=' + GlobalComment + '&WorkflowId=' + WorkflowId + '&StepId=' + StepId + '&StepParticipantActionId=' + StepParticipantActionId;
//            break;
//        case "Change":
//            window.location.href = '/GenericGrid/Change?ActionName=' + ActionName + '&TransactionId=' + TransactionId + '&Comments=' + GlobalComment + '&WorkflowId=' + WorkflowId + '&StepId=' + StepId + '&Source=' + Source + '&StepParticipantActionId=' + StepParticipantActionId;
//            break;

//        }
//    */
//}
function FnClickSecondaryButtons(ActionName, WorkflowId, StepId, Source, TransactionId, GlobalComment, StepParticipantActionId, ActionUrl) {
    //replace \n characters in comments with @ as \n is being dropped on reaching to controller
    var Comments = GlobalComment.split('\n').join('@\n');
    GlobalComment = Comments;
    //code generalized
    var UrlString = "";
    if (ActionUrl.indexOf("UpdateActionStatus") > -1 || ActionUrl.indexOf("ResetPassword") > -1) {
        //UrlString = ActionUrl + "?TransactionId=" + TransactionId + "&Comments=" + GlobalComment + "&Source=" + Source + "&StepParticipantActionId=" + StepParticipantActionId;
        var yourData = {
            TransactionId: TransactionId.join(','),
            Comments: GlobalComment,
            Source: Source,
            StepParticipantActionId: StepParticipantActionId
        };
        var loader; // Declare the loader variable outside the AJAX call
        $.ajax({
            url: ActionUrl,
            type: "POST",
            data: JSON.stringify(yourData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                loader = $('<div style="position: fixed; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 9999;background-color:#F5F5F5;border-radius: 5px;"><div style="float: left; overflow: hidden; width: 32px; height: 32px;" class="jqx-grid-load"></div><span style="margin-top: 10px; float: left; display: block; margin-left: 5px;">Loading...</span></div>'); // Create the loader
                $('body').append(loader); // Add the loader to the body
                //alert('sending data using mass action');
            },
            success: function (response) {
                loader.remove(); // Remove the specific loader
                if (response.success) {
                } else {

                    alert(response.message);
                }
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                }
            }
        });
    }
    else {
        debugger;
        UrlString = ActionUrl + "/" + TransactionId + "?FormType=" + ActionName + "&Source=" + Source;
        window.location.href = UrlString;
    }
}


function FnReplaceAttributeValues(p_CheckBoxAttributeValues, KeyForReplacement, ValueTobeReplaced) {
    if (p_CheckBoxAttributeValues != null && p_CheckBoxAttributeValues != 'undefined') {
        var TotaLlength = p_CheckBoxAttributeValues.length;
        var l_arAttributes = p_CheckBoxAttributeValues.split('|');
        for (var l_nIndex = 0; l_nIndex < l_arAttributes.length; ++l_nIndex) {
            var l_arAttr = l_arAttributes[l_nIndex].split(':');
            if (l_arAttr[0] == KeyForReplacement) {
                l_arAttr[1] = ValueTobeReplaced;
                l_arAttributes[l_nIndex] = l_arAttr.join(':');
                p_CheckBoxAttributeValues = l_arAttributes.join('|');
                break;
            }
        }
    }
    return p_CheckBoxAttributeValues;
}


function GenerateDisabledCompanySpecificFormElements(CompanySpecificArray, FormType, TransactionId) {

    //Order the form field as per ordinal number of Companyspecificdata
    // console.log(CompanySpecificArray);
    /*Loop through company specific data and add divs on screen as per ordinal number of the field*/
    var ElementToBeAdded = '';
    var body = document.getElementById('DIVAttributeContainer');
    //As ProductPob is contained within Product form, both of them contain attribute columns and need to populate in separate div.
    if (String(FormType) == 'LProductPobs') {
        body = document.getElementById('DIVProductPOBAttributeContainer');
    }
    for (var i = 0; i < CompanySpecificArray.length; ++i) {
        var DefaultValue = CompanySpecificArray[i].DefaultValue;

        //check on the basis of datatype
        //creating DropDown/Textbox for string type attributes
        if (String(CompanySpecificArray[i].DataType) == 'nvarchar') {
            if (DefaultValue == null) {
                DefaultValue = '';
            }
            //create parent div
            var newdiv = document.createElement('div');   //create a div
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            body.insertBefore(newdiv, body.lastChild);

            // Creating label
            var newlabel = document.createElement("Label");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);

            var subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);

            //if dropdown Id is available, then create DropDown Box,else create Editor
            if (CompanySpecificArray[i].DropDownId) {
                var SelectElement = document.createElement("select");
                if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    SelectElement.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                    SelectElement.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
                } else {
                    SelectElement.id = CompanySpecificArray[i].ColumnName;
                    SelectElement.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
                // SelectElement.id = CompanySpecificArray[i].ColumnName;

                SelectElement.setAttribute("class", "form-control input-sm");
                SelectElement.setAttribute("maxlength", CompanySpecificArray[i].MaximumLength);
                // SelectElement.setAttribute("value",DefaultValue); 
                SelectElement.disabled = true;
                SelectElement.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(SelectElement);
            }
            else if (CompanySpecificArray[i].IsMultiline) {
                var element = document.createElement("input");
                element.setAttribute("type", "textarea");
                if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    element.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                    element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);

                } else {
                    element.id = CompanySpecificArray[i].ColumnName;
                    element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
                // element.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                // element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                element.setAttribute("maxlength", CompanySpecificArray[i].MaximumLength);
                //element.setAttribute("value",DefaultValue); 
                element.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(element);
                element.setAttribute("class", "form-control");
                element.setAttribute("class", "textarea1");
                element.disabled = true;
                element.setAttribute("style", "min-height:50px;");

            }
            else {
                var element = document.createElement("input");
                element.setAttribute("type", "text");
                if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    element.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                    element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
                } else {
                    element.id = CompanySpecificArray[i].ColumnName;
                    element.setAttribute("name", CompanySpecificArray[i].ColumnName);
                }
                element.setAttribute("class", "form-control input-sm");
                element.setAttribute("maxlength", CompanySpecificArray[i].MaximumLength);
                // element.setAttribute("value",DefaultValue);
                element.disabled = true;
                element.innerHTML = CompanySpecificArray[i].ColumnName;
                subDiv.appendChild(element);
            }
            //Validation Message display by below code
            var Validateelement = document.createElement("span");
            Validateelement.setAttribute("class", "field-validation-valid text-danger");
            if (String(FormType) == 'LProductPobs') {
                Validateelement.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                Validateelement.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

            }
            Validateelement.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(Validateelement);

        }
        //create input type number for numeric types
        if ((String(CompanySpecificArray[i].DataType) == 'numeric')) {

            //create parent div
            var newdiv = document.createElement('div');   //create a div
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            body.insertBefore(newdiv, body.lastChild);

            // Creating label
            var newlabel = document.createElement("Label");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);

            var subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);


            var element = document.createElement("input");
            element.setAttribute("type", "number");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                element.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.id = CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", CompanySpecificArray[i].ColumnName);
            }
            element.setAttribute("class", "form-control input-sm");
            // element.setAttribute("value",DefaultValue);
            element.disabled = true;
            element.innerHTML = CompanySpecificArray[i].ColumnName;
            subDiv.appendChild(element);
            //Validation Message display by below code
            element = document.createElement("span");
            element.setAttribute("class", "field-validation-valid text-danger");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names 
                element.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

            }
            element.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(element);
        }

        //create input type number for int types
        if ((String(CompanySpecificArray[i].DataType) == 'int')) {
            //create parent div
            var newdiv = document.createElement('div');   //create a div
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            body.insertBefore(newdiv, body.lastChild);

            // Creating label
            var newlabel = document.createElement("Label");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);

            var subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);


            var element = document.createElement("input");
            element.setAttribute("type", "number");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                element.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.id = CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", CompanySpecificArray[i].ColumnName);
            }
            element.setAttribute("class", "form-control input-sm");
            // element.setAttribute("value",DefaultValue);
            element.disabled = true;
            element.innerHTML = CompanySpecificArray[i].ColumnName;
            var elementId = CompanySpecificArray[i].ColumnName;
            element.onblur = function (elementId) {
                var sourceElement = elementId.currentTarget.id;
                var val = document.getElementById("" + sourceElement).value;
                var countDot = (val.match(/\./g) || []).length;
                if (countDot > 0) {
                    //  alert("Decimal Point not allowed.");
                    document.getElementById("" + sourceElement).value = '';
                    return;
                }
            };
            subDiv.appendChild(element);
            //Validation Message display by below code
            element = document.createElement("span");
            element.setAttribute("class", "field-validation-valid text-danger");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names 
                element.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

            }
            element.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(element);
        }
        //create checkboxes for Bit type attributes
        if (String(CompanySpecificArray[i].DataType) == 'bit') {
            newdiv = document.createElement('div');   //create a div for CheckBox
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("style", "min-height:80px;");
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            body.appendChild(newdiv);
            //Create Label for CheckBox
            newlabel = document.createElement("Label");

            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(CompanySpecificArray[i].ColumnName));
            }
            //newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
            newlabel.setAttribute("class", "col-md-5 col-md-5 rely-labels");
            //newlabel.setAttribute("style", "text-align: right;");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);

            subDiv = document.createElement('div');   //create a sub div for textbox
            subDiv.setAttribute("class", "col-md-7 col-lg-7");
            newdiv.appendChild(subDiv);
            //create checkbox
            element = document.createElement("input");
            element.setAttribute("type", "checkbox");
            element.disabled = true;
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                element.id = String(FormType) + CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.id = CompanySpecificArray[i].ColumnName;
                element.setAttribute("name", CompanySpecificArray[i].ColumnName);

            }
            //element.id = "Lbl" + CompanySpecificArray[i].ColumnName;

            // element.setAttribute("class", "form-control");
            element.innerHTML = CompanySpecificArray[i].ColumnName;
            var elementId = CompanySpecificArray[i].ColumnName;
            element.onclick = function (elementId) {
                var sourceElement = elementId.currentTarget.id;
                var val = document.getElementById("" + sourceElement + "").checked ? true : false;
                CheckBoxAttributeValues = FnReplaceAttributeValues(CheckBoxAttributeValues, sourceElement, val);
            };

            subDiv.appendChild(element);
            //Validation Message display by below code
            element = document.createElement("span");
            element.setAttribute("class", "field-validation-valid text-danger");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names 
                element.setAttribute("data-valmsg-for", String(FormType) + CompanySpecificArray[i].ColumnName);
            } else {
                element.setAttribute("data-valmsg-for", CompanySpecificArray[i].ColumnName);

            }
            element.setAttribute("data-valmsg-replace", "true");
            subDiv.appendChild(element);
            // subDiv.appendChild'<span class="field-validation-valid text-danger" data-valmsg-for="'+element.id+'" data-valmsg-replace="true"> </span>'
        }
        //create DateTime for Date type attributes
        if (String(CompanySpecificArray[i].DataType) == 'Datetime' || String(CompanySpecificArray[i].DataType) == 'datetime' || String(CompanySpecificArray[i].DataType) == 'date') {
            newdiv = document.createElement('div');   //create a div for CheckBox
            newdiv.id = 'DIV' + String(CompanySpecificArray[i].ColumnName);
            newdiv.setAttribute("class", "col-md-3 col-lg-3");//SS changed the col-md-4 col-lg-4 to col-md-3 col-lg-3
            newdiv.setAttribute("style", "min-height:80px;");
            body.appendChild(newdiv);
            //Create Label for CheckBox
            newlabel = document.createElement("Label");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                newlabel.id = "Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(FormType) + String(CompanySpecificArray[i].ColumnName));
            } else {
                newlabel.id = "Lbl" + CompanySpecificArray[i].ColumnName;
                newlabel.setAttribute("for", String(CompanySpecificArray[i].ColumnName));
            }
            newlabel.setAttribute("class", "col-md-11 col-md-11 rely-labels");//SS changed this line
            newlabel.setAttribute("style", "text-align: right");
            newlabel.innerHTML = CompanySpecificArray[i].Label;
            newdiv.appendChild(newlabel);
            subDiv = document.createElement('div');
            subDiv.setAttribute("class", "col-md-11 col-lg-11");
            newdiv.appendChild(subDiv);
            subDiv1 = document.createElement('div');   //create a sub div
            if (String(FormType) == 'LProductPobs') {
                subDiv1.id = String(FormType) + CompanySpecificArray[i].ColumnName;
            } else {
                subDiv1.id = CompanySpecificArray[i].ColumnName;
            }
            subDiv.appendChild(subDiv1);
            
            //need to create Datetime picker here
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                $("#" + String(FormType) + CompanySpecificArray[i].ColumnName).jqxDateTimeInput({ width: '200px', height: '30px', placeHolder: "DD/MM/YYYY", value: null });
            } else {
                $("#" + CompanySpecificArray[i].ColumnName).jqxDateTimeInput({ width: '200px', height: '30px', placeHolder: "DD/MM/YYYY", value: null });
            }
            $("#" + CompanySpecificArray[i].ColumnName).jqxDateTimeInput({ disabled: false });
        }


        //Make a server call to get dropdown values to be displayed if dropdownId is present in that column
        if (CompanySpecificArray[i].DropDownId) {
            var DropdownId = Number(CompanySpecificArray[i].DropDownId);
            var ColumnName = CompanySpecificArray[i].ColumnName;
            FnFillDropDown(DropdownId, ColumnName, TransactionId, FormType, DefaultValue);

        }
        //Add tooltip which is same as text box value as directed by JS
        if (document.getElementById(String(CompanySpecificArray[i].ColumnName))) {


            var TooltipValue = "";
            if ($('#' + String(CompanySpecificArray[i].ColumnName)).is("select")) {
                // the input field is  a dropdown
                TooltipValue = $("#" + String(CompanySpecificArray[i].ColumnName) + " option:selected").text();
            }
            else {
                TooltipValue = document.getElementById(String(CompanySpecificArray[i].ColumnName)).value;
            }
            //document.getElementById(String(CompanySpecificArray[i].ColumnName)).title = TooltipValue;
        }

        //check if IsManadatory is true then dynamically add validation to those fields
        if (Boolean(CompanySpecificArray[i].IsMandatory) == true) {

            if (String(CompanySpecificArray[i].DataType) == 'nvarchar') {
                if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                    if (document.getElementById(String(FormType) + CompanySpecificArray[i].ColumnName)) {
                        $('#' + String(FormType) + String(CompanySpecificArray[i].ColumnName)).prop('required', true);
                    }
                } else {
                    if (document.getElementById(CompanySpecificArray[i].ColumnName)) {
                        $('#' + String(CompanySpecificArray[i].ColumnName)).prop('required', true);
                    }
                }

            }
            //Add red star on required columns
            //$("#Lbl" + CompanySpecificArray[i].ColumnName).addClass("required");
            if (String(FormType) == 'LProductPobs') { //Attribute columns are common in Product and ProductPob divs, therefore segregating them through diff names
                $("#Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName).addClass("required");

            } else {
                $("#Lbl" + CompanySpecificArray[i].ColumnName).addClass("required");
            }
        }

        //replace custom label with Default Label
        if (String(FormType) == 'LProductPobs') {
            if (CompanySpecificArray[i].Label && document.getElementById("Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName)) {
                document.getElementById("Lbl" + String(FormType) + CompanySpecificArray[i].ColumnName).innerHTML = CompanySpecificArray[i].Label;
            }
        }
        else {
            if (CompanySpecificArray[i].Label && document.getElementById("Lbl" + CompanySpecificArray[i].ColumnName)) {
                document.getElementById("Lbl" + CompanySpecificArray[i].ColumnName).innerHTML = CompanySpecificArray[i].Label;
            }
        }


        //Where(p => p.ColumnName == "PayeeId").Where(p => p.DisplayOnForm == true)
        if (Boolean(CompanySpecificArray[i].DisplayOnForm) == true) {
            if (document.getElementById('DIV' + CompanySpecificArray[i].ColumnName)) {
                ElementToBeAdded += document.getElementById('DIV' + CompanySpecificArray[i].ColumnName).outerHTML;
            }
        }

    }
    /*Now remove the UnOrdered Div from the screen*/
    $('#UnOrderedForm').empty();
    /*Add Ordered Div on the Form*/
    $('#OrderedForm').html(ElementToBeAdded);

}