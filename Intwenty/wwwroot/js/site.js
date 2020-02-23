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
    getDataViewValue(keyfield, lookupid, senddata) {

        var context = this.vuecontext;

        $("input[data-lookupid]").each(function () {
            var id = $(this).data('lookupid');
            if (id === lookupid) {
                var dbfield = $(this).data('dbfield');
                if (dbfield === keyfield) {
                    senddata.SearchValue = context.model[dbfield];
                }
            }
        });

        if (!senddata.SearchValue)
            return;


        $.ajax({
            url: this.endpoint,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(senddata),
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


}