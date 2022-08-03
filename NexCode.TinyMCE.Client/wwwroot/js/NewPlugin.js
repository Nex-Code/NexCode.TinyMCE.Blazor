

export function showDialog(editorId, tokens) {
    var editor = tinymce.get(editorId);

    editor.windowManager.open({
        title: 'Example plugin',
        body: {
            type: 'panel',
            items: [
                {
                    type: 'grid',
                    columns: 3,
                    items: [
                        {
                            type: 'button',
                            text: 'title',
                            name:'value'
                        },
                        {
                            type: 'button',
                            text: 'title',
                        },
                        {
                            type: 'button',
                            text: 'title',
                        }

                    ]
                }
            ]
        },
        buttons: [
            {
                type: 'submit',
                text: 'Close'
            }
        ],
        onAction: function(api, d, t, y) {

        },
        onSubmit: function (api) {
            var data = api.getData();
            // Insert content when the window fsorm is submitted #1#
            editor.insertContent('Title: ' + data.title);
            api.close();
        }
    });
}