        //---------------------------------------------------------------------------------------------------------------------
        //----------------------------------------Number Formatter by ProWebMasters.net----------------------------------------
        //----------------------------------------  You are free to use this in your   ----------------------------------------
        //----------------------------------------  scripts, but you must leave this   ----------------------------------------
        //----------------------------------------   Copyright notice intact. Enjoy!   ----------------------------------------
        //---------------------------------------------------------------------------------------------------------------------
        //---------------------------------------- You can call this function with only----------------------------------------
        //----------------------------------------   the 'num' object (reference your  ----------------------------------------
        //----------------------------------------   field you want to format). If no  ----------------------------------------
        //----------------------------------------formats are sent, it will use default----------------------------------------
        //----------------------------------------             formats.                ----------------------------------------
        //---------------------------------------------------------------------------------------------------------------------
        function FormatNumber(num, format, shortformat) {
            if (format == null) {
                // Choose the default format you prefer for the number.
                //format = "#-(###) ###-#### ";		// Telephone w/ LD Prefix and Area Code
                format = "(###) ###-#### "; 		// Telephone w/ Area Code
                //format = "###-###-####";			// Telephone w/ Area Code (dash seperated)
                //format = "###-##-####";			//Social Security Number
            }
            //---------------------------------------------------------------------------------------------------------------------
            //---------------------------------------------------------------------------------------------------------------------
            if (shortformat == null) {
                // Choose the short format (without area code) you prefer.
                //If you do not want multiple formats, leave it as "".

                //var shortformat = "###-#### ";
                var shortformat = "";
            }

            //---------------------------------------------------------------------------------------------------------------------
            //----------------------------------------This code can be used to format any number. ---------------------------------
            //----------------------------------------Simply change the format to a number format ---------------------------------
            //---------------------------------------- you prefer. It will ignore all characters  ---------------------------------
            //----------------------------------------  except the #, where it will replace with  ---------------------------------
            //----------------------------------------               user input.                  ---------------------------------
            //---------------------------------------------------------------------------------------------------------------------

            var validchars = "0123456789";
            var tempstring = "";
            var returnstring = "";
            var extension = "";
            var tempstringpointer = 0;
            var returnstringpointer = 0;
            count = 0;

            // Get the length so we can go through and remove all non-numeric characters
            var length = num.value.length;


            // We are only concerned with the format of the phone number - extensions can be left alone.
            if (length > format.length) {
                length = format.length;
            };

            // scroll through what the user has typed
            for (var x = 0; x < length; x++) {
                if (validchars.indexOf(num.value.charAt(x)) != -1) {
                    tempstring = tempstring + num.value.charAt(x);
                };
            };
            // We should now have just the #'s - extract the extension if needed
            if (num.value.length > format.length) {
                length = format.length;
                extension = num.value.substr(format.length, (num.value.length - format.length));
            };

            // if we have fewer characters than our short format, we'll default to the short version.
            for (x = 0; x < shortformat.length; x++) {
                if (shortformat.substr(x, 1) == "#") {
                    count++;
                };
            }
            if (tempstring.length <= count) {
                format = shortformat;
            };


            //Loop through the format string and insert the numbers where we find a # sign
            for (x = 0; x < format.length; x++) {
                if (tempstringpointer <= tempstring.length) {
                    if (format.substr(x, 1) == "#") {
                        returnstring = returnstring + tempstring.substr(tempstringpointer, 1);
                        tempstringpointer++;
                    } else {
                        returnstring = returnstring + format.substr(x, 1);
                    }
                }

            }

            // We have gone through the entire format, let's add the extension back on.
            returnstring = returnstring + extension;

            //we're done - let's return our value to the field.
            num.value = returnstring;
        }



        function CheckDate( sample){
             if (sample.length < 10) {

                        var dateSplit = sample.split("/");
                        //alert(dateSplit.length);
                        var prevent = false;
                        if (dateSplit.length < 3)
                            prevent = true;
                        else if (dateSplit[2].length < 4)
                            prevent = true;
                        else {
                            if (dateSplit[0].length == 1)
                                dateSplit[0] = "0" + dateSplit[0];

                            if (dateSplit[1].length == 1)
                                dateSplit[1] = "0" + dateSplit[1];
                        }



                        var newDate = dateSplit.join('/');
                        //alert(newDate);
                        $(this).val(newDate);
                        //alert($(this).val().length);

                        if (newDate.length < 10)
                            prevent = true;


                    }

                    if (!prevent) {
                        var comp = sample.split("/");
                        var m = parseInt(comp[0], 10);
                        var d = parseInt(comp[1], 10);
                        var y = parseInt(comp[2], 10);
                        var date = new Date(y, m - 1, d);
                        if (date.getFullYear() == y && date.getMonth() + 1 == m && date.getDate() == d) {
                           // alert('Valid date');
                        } else {
                          //  alert('Invalid date');
                            prevent = true;
                        }
                    }
                    return prevent;
        }


        jQuery.browser = {};
        jQuery.browser.mozilla = /mozilla/.test(navigator.userAgent.toLowerCase()) && !/webkit/.test(navigator.userAgent.toLowerCase());
        jQuery.browser.webkit = /webkit/.test(navigator.userAgent.toLowerCase());
        jQuery.browser.opera = /opera/.test(navigator.userAgent.toLowerCase());
        jQuery.browser.msie = /msie/.test(navigator.userAgent.toLowerCase());
