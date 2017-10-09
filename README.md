# Episerver Markup

A library to assist working with "hand-coded" markup in Episerver CMS.

It provides three content types:

* Markup Block
* Markup File
* Markup Archive File

Content representing hand-coded markup can be dragged into a content area where it will output to the page along with associated "resources", meaning inline or references scripts and styles.

This markup is either manually added in Edit Mode (through a `MarkupBlock`) or added via file upload (`MarkupFile` or `MarkupArchiveFile`).

>Note: this code will not compile without adding local references to `Episerver.dll`, `Episerver.Data.dll`, `Episerver.Shell`, and `Episerver.Framework.dll`. This code has been compiled against v10.10.3 of those assemblies.

## Content Types

This library provides three content types, all of which implement a common interface and result in the same basic behavior: markup output to the page, with associated styles and scripts either added inline or referenced from URLs.

### Interface: IMarkupContent

The `IMarkupContent` interface provides for five properties which output content to the page

**string Markup**    
Handled differently in the different implementations. Available in Edit Mode for `MarkupBlock` and read from the associated media for `MarkupFile` and `MarkupArchiveFile`.

**string InlineStyles**   
Raw CSS to be output to the page on which the content is placed. This will be added via `ClientResources.RequireStyleInline()`. Available in Edit Mode.

**string InlineScript**   
Raw Javascript to be output to the page on which the content is placed. This will be added via `ClientResources.RequireScriptInline()`. Available in Edit Mode.

**string StylesheetReferences**    
URLs of stylesheets to be including on the page via a `LINK` tag (one URL per line). These will be added via to `ClientResources.RequireStyle()`. Available in Edit Mode.

**string ScriptReferences**    
URLs of script files to be including on the page via a `SCRIPT` tag (one URL per line). These will be added via `ClientResources.RequireScript().AtFooter()`. Available in Edit Mode.

By default, the Alloy demo site places references --

* Styles: in the `HEAD` tag
* Scripts: just above the closing `BODY` tag

See the `OnBeforeAddReference` event below for other ways of handling their placement.

Managed references (meaning references that map to content objects), will be routed through a resource handler class. By default, this path is:

    /markup.resource?id=[ID of the content]&file=[name of the file]

The resource handler reads the specified content object and extracts and returns the resource contents.

All of the properties above use an instance of the "Poor Man's Code Editor" for their UI editing component.

https://gist.github.com/deanebarker/f1c2542b3a510eb992c76c7e07c2f16b

### BlockData: Markup Block

Provides an explicit `Markup` field for raw HTML.

### MediaData: Markup File

Mapped to the `.html` and `.htm` extensions by default. Represents an HTML file. When placed on a page, it will write its contents to the response.

The markup is the content of the file itself.

All files in the same asset folder as the Markup File will be evaluated as a resource. Anything with a `.js` or `.css` extensions will be added as references. (Clearly, put a `MarkupFile` in its own asset folder, unless more than one file should share the same script and style resources.)

### MediaData: Markup Archive File

Mapped to the `.app` extension by default. Represents a zip archive with its extension changed. Related markup assets can be placed in this archive so they can be managed as a single file.

The markup is an HTML file in the _root_ of the archive with the same base name as the archive and a `.html` extension. (e.g. -- if the archive is called `markup.app` then the file in the root should be `markup.html`)

Any `.js` and `.css` files in the _root_ of this archive will be added as references.

Note the emphasis on _root_. This means you can't zip a directory because when those files will be in the directory in the root. On Windows, this means you highlight a group of files (_not_ a directory), then select `Send To...Compressed (zipped) folder`.

A video of the Markup Archive File in action is available here:

https://vimeo.com/236467181

## Available Events

**MarkupEventManager.OnBeforeOutputMarkup(object, MarkupEventArgs)**    
Allows modification of markup before output. Markup is in `e.Text` and can be modified in place.

**MarkupEventManager.OnBeforeOutputStylesheet(object, MarkupEventArgs)**    
Allows modification of stylesheet content output. CSS is in `e.Text` and can be modified in place.

**MarkupEventManager.OnBeforeOutputScript(object, MarkupEventArgs)**    
Allows modification of script content before output. Script is in `e.Text` and can be modified in place.

**MarkupEventManager.OnBeforeAddReference(object, MarkupReferenceEventArgs)**    
Allows cancellation of CSS/JS prior to reference inclusion. Set `e.Cancel` to true to cancel the inclusion. This can be useful if you want your references somewhere other than the default location -- in the event handler, just cancel the inclusion and handle them however you like.

## Status

**Ridiculously Alpha**. This is something I was merely playing around with. It works in my limited demo environment, but it has not gone through any formal testing whatsoever.

>Deane Barker    
deane@blendinteractive.com    
@gadgetopia