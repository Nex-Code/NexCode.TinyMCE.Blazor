# NexCode.TinyMCE.Blazor
A Blazor wrapper for TinyMCE self hosted

Add the following to your startup:

    builder.Services.AddTinyMCE();
    
Add the following to _Imports.razor, or as the @using on the page

    @using NexCode.TinyMCE.Blazor


Then you can use

```
<RichTextEditor/>
<RichTextEditor>
	{content}
</RichTextEditor>
```

There's support for the Open Source plugins: https://www.tiny.cloud/docs/plugins/
Use the Plugins, Toolbar, and MenuBar parameters or the Options parameter to customise the editor.
