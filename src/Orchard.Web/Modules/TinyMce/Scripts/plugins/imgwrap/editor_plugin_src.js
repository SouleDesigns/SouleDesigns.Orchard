
(function () {

    ////////////////////////////////////////////////////////////////////////
    // NOTE: IF YOU EDIT THIS FILE
    // You must also update editor_plugin.js with a minified version.
    ////////////////////////////////////////////////////////////////////////
    tinymce.create('tinymce.plugins.Orchard.ImgWrap', {
        /**
        * Initializes the plugin, this will be executed after the plugin has been created.
        * This call is done before the editor instance has finished it's initialization so use the onInit event
        * of the editor instance to intercept that event.
        *
        * @param {tinymce.Editor} ed Editor instance that the plugin is initialized in.
        * @param {string} url Absolute URL to where the plugin is located.
        */
        init: function (ed, url) {
            // Register the command so that it can be invoked by using tinyMCE.activeEditor.execCommand('mceMediaPicker');
            ed.addCommand('mceImgWrap', function () {
                ed.focus();
                var selectedText = ed.selection.getContent({ format: 'html' });                
                var wrappedText = selectedText;

                if (selectedText != undefined && selectedText != "") {
                    var selectedNode = $(selectedText);
                    var caption = '';

                    // Check for existing wrapper
                    if(selectedNode.parents(".tr-caption-container").length <= 0 )
                    {
                        // Create popup if it doesn't exist
                        if ($("#imgwrap-popup").length <= 0) {
                            $('<div id="imgwrap-popup" style="padding: 20px;">' +
                                '<h2>Image Wrap!</h2>' +
                                '<label for="imgwrap-caption">Caption</label>' +
                                '<input type="text" id="imgwrap-caption" /><br /><br />' +
                                '<input type="button" value="Wrap It!" ' + 
                                    'onclick="$.colorbox.doWrapIt = true; $.colorbox.close();" /></div>')
                                .appendTo("body");
                        }

                        // Assure return value is false
                        $.colorbox.doWrapIt = false;

                        // Show box
                        $.colorbox({
                            inline: true,
                            href: "#imgwrap-popup",
                            height: '600px',
                            width: '600px',
                            onClosed: function (v) {
                                if ($.colorbox.doWrapIt) {
                                    // Wrapper mimics blogger format
                                    wrappedText =
                                    '<table align="center" cellpadding="0" cellspacing="0" class="tr-caption-container" style="margin-left: auto; margin-right: auto; text-align: center;">' +
                                        '<tbody>' +
                                        '<tr>' +
                                            '<td style="text-align: center;">' +
                                                '<a href="' + selectedNode.attr('src') + '" imageanchor="1" style="margin-left: auto; margin-right: auto;">' +
                                                selectedText +
                                                '</a>' +
                                            '</td>' +
                                        '</tr>' +
                                        '<tr>' +
                                            '<td class="tr-caption" style="text-align: center;">' + $("#imgwrap-caption").val() + '</td>' +
                                        '</tr>' +
                                        '</tbody>' +
                                    '</table>';


                                    // reassign the src to force a refresh
                                    tinyMCE.execCommand('mceReplaceContent', false, wrappedText);
                                }
                            }
                        });                       
                    }                                      
                }

                                   
            });

            // Register media button
            ed.addButton('imgwrap', {
                title: 'Wrap Image Like Blogger!', 
                cmd: 'mceImgWrap',
                image: url + '/img/img_wrap.png'
            });
        },

        /**
        * Creates control instances based in the incomming name. This method is normally not
        * needed since the addButton method of the tinymce.Editor class is a more easy way of adding buttons
        * but you sometimes need to create more complex controls like listboxes, split buttons etc then this
        * method can be used to create those.
        *
        * @param {String} n Name of the control to create.
        * @param {tinymce.ControlManager} cm Control manager to use inorder to create new control.
        * @return {tinymce.ui.Control} New control instance or null if no control was created.
        */
        createControl: function (n, cm) {
            return null;
        },

        /**
        * Returns information about the plugin as a name/value array.
        * The current keys are longname, author, authorurl, infourl and version.
        *
        * @return {Object} Name/value array containing information about the plugin.
        */
        getInfo: function () {
            return {
                longname: 'Orchard Imge Wrap Plugin',
                author: 'Owen Soule',
                authorurl: 'http://www.souledesigns.com',
                infourl: 'http://www.souledesigns.com',
                version: '1.0'
            };
        }
    });

    // Register plugin
    tinymce.PluginManager.add('imgwrap', tinymce.plugins.Orchard.ImgWrap);

})();