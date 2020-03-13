﻿/*
 * Helper for Intwenty javascript / vue 
 */

class IntwentyHelper
{

    constructor(vuecontext, endpoint)
    {
        this.vuecontext = vuecontext;
        this.endpoint = endpoint;
    }

    /*
     * Function to retrieve a dataview value (record) 
     * Called from views: create/open
     * Usage: Wxwcuted when a user tabs out of an input LOOKUPKEYFIELD included in a LOOKUP
    */
    getDataViewValue(keyfield, lookupid) {

        var context = this.vuecontext;

        $("input[data-lookupid]").each(function () {
            var id = $(this).data('lookupid');
            if (id === lookupid) {
                var dbfield = $(this).data('dbfield');
                var viewfield = $(this).data('viewfield');
                if (dbfield === keyfield)
                {
                    context.viewretrieveinfo.filterField = viewfield;
                    context.viewretrieveinfo.filterValue = context.model[dbfield];
                }
            }
        });

        if (!context.viewretrieveinfo.filterValue)
            return;
        if (context.viewretrieveinfo.filterValue.length == 0)
            return;


        $.ajax({
            url: this.endpoint,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(context.viewretrieveinfo),
            success: function (response) {
                var dataviewitem = JSON.parse(response.data);
                $("input[data-lookupid]").each(function () {
                    var id = $(this).data('lookupid');
                    if (id === lookupid) {
                        var dbfield = $(this).data('dbfield');
                        var viewfield = $(this).data('viewfield');
                        context.model[dbfield] = dataviewitem[viewfield];
                        context.$forceUpdate();
                    }
                });


            }
        });

    }

    /*
     * Function to set selected value from dataview to a application table
     * Called from views: create/open
     * 
     */
    setSelectedDataViewValue(item, lookupid)
    {
        var context = this.vuecontext;

        $("input[data-lookupid]").each(function ()
        {
            var id = $(this).data('lookupid');
            if (id === lookupid) {
                var dbfield = $(this).data('dbfield');
                var viewfield = $(this).data('viewfield');
                context.model[dbfield] = item[viewfield];
            }
        });

        context.$forceUpdate();
    }

    getDataViewLookUpPage()
    {
        var context = this.vuecontext;

        $.ajax({
            url: this.endpoint,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(context.viewretrieveinfo),
            success: function (response)
            {
                //DATA
                context.dataview = JSON.parse(response.data);

                //UPDATE CURRENT PAGE INFO
                context.viewretrieveinfo = response.retriveListArgs;
            }
        });
    }

    /*
     * Handles userinput
     * 
     */
    onUserInput(event)
    {

        if (!event)
            return;

        var elementId = event.srcElement.id;
        if (!elementId)
            return;

        //Remove requiredNotValid if the input is valid
        $("[data-required]").each(function () {
            var required = $(this).data('required');
            var id = $(this).attr('id');
            if (required === "True" && id === elementId) {
                var val = event.srcElement.value;
                if (val) {
                    if (val.length > 0)
                        $("#" + elementId).removeClass('requiredNotValid');
                }
            }
        });


    }

    /*
     * Client validation.
     * Check if this application can be changed.
     */ 
    canSave()
    {
        var context = this.vuecontext;
        var result = true;
        $("[data-required]").each(function () {
            var required = $(this).data('required');
            if (required === "True") {
                var dbfield = $(this).data('dbfield');
                if (!context.model[dbfield]) {
                    result = false;
                    $(this).addClass('requiredNotValid');
                }
                else if (context.model[dbfield].length == 0) {
                    result = false;
                    $(this).addClass('requiredNotValid');
                }
                else {
                    $(this).removeClass('requiredNotValid');
                }
            }
        });


        return result;
    }


}

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