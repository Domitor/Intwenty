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

        $("input[data-lookupid]").each(function () {
            var id = $(this).data('lookupid');
            if (id === lookupid) {
                var dbfield = $(this).data('dbfield');
                if (dbfield === keyfield) {
                    senddata.SearchValue = this.vuecontext.model[dbfield];
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
                        this.vuecontext.model[dbfield] = dataviewitem[viewfield];
                        this.vuecontext.$forceUpdate();
                    }
                });


            }
        });

    }
}