

function raiseValidationErrorModal(message)
{
    $('#msg_dlg_modal_hdr').text('Error');
    $('#msg_dlg_modal_text').text(message);
    $('#msg_dlg_modal').modal();

}

function raiseErrorModal(operationresult)
{
    $('#msg_dlg_modal_hdr').text('Error');
    $('#msg_dlg_modal_text').text(operationresult.userError);
    $('#msg_dlg_modal').modal();

}

function raiseYesNoModal(headertxt, bodytext, yes_callback)
{
    $('#yesno_dlg_modal_hdr').text(headertxt);
    $('#yesno_dlg_modal_text').text(bodytext);
    $('#yesno_dlg_modal_yesbtn').click(yes_callback);
    $('#yesno_dlg_modal').modal();

}

function hasRequiredValues(datalist, requiredlist)
{

    for (var i = 0; i < datalist.length; i++)
    {
        for (var z = 0; z < requiredlist.length; z++)
        {
            var fld = requiredlist[z];
            if (!datalist[i][fld])
                return false;
            if (!datalist[i][fld] === "")
                return false;

        }          
    }

    return true;

}