# Episerver Markup

A library to assist working with "hand-coded" markup in Episerver CMS.

It provides three content types:

* Markup Block
* Markup File
* Markup Archive File

Content representing hand-coded markup can be dragged into a content area where it will output to the page along with associated "resources", meaning inline or referenced scripts and styles.

Markup can be either manually added in Edit Mode (through a `MarkupBlock`) or added via file upload (`MarkupFile` or `MarkupArchiveFile`).

>Note: this code will not compile without adding local references to `Episerver.dll`, `Episerver.Data.dll`, `Episerver.Shell`, and `Episerver.Framework.dll`. This code has been compiled against v10.10.3 of those assemblies.

Each content type logically consists of four basic elements:

* **Base Markup.** This is the markup that is output to the page.
* **Inline Scripts/Styles.** This is inline Javascript and CSS that is directly output to the page.
* **Local Resources.** These are Javascript and CSS files associated with attached to the content (through various methods, described below) that are referenced through `LINK` and `SCRIPT` tags.
* **Remote Resource URLs.** These are URLs to off-site Javascript and CSS that are referenced through `LINK` and `SCRIPT` tags. (i.e. -- jQuery, Bootstrap, etc.)

Base Markup, Inline Scripts, and Inline Styles are always output to the page. Local and Remote Resources are evaluated for reference inclusion.

(To clarify: a _resource_ is the file itself, a _reference_ is the tag that invokes that file. So, a CSS file is a _resource_. The `LINK` tag that loads that file for a page is a _reference_.)

References to Local Resources are placed using the `ClientResources` helper class.

By default, the Alloy demo site places these references --

* Styles: in the `HEAD` tag
* Scripts: just above the closing `BODY` tag

Your templating may vary. See the `OnBeforeAddReference` event below for how to override reference placement.

Managed resources (meaning references that map to resources associated with content objects), will be routed through a resource handler class. By default, this path is:

    /markup.resource?id=[ID of the content]&file=[name of the file]

The resource handler reads the specified content object and extracts and returns the resource contents.

All of the properties above use an instance of the "Poor Man's Code Editor" for their UI editing component (when/if available in Edit Mode).

https://gist.github.com/deanebarker/f1c2542b3a510eb992c76c7e07c2f16b

## Content Types

There are three content types -- a block, and two media items. All implement the `IMarkupContent` interface.

### Interface: IMarkupContent

The `IMarkupContent` interface provides for the following. Items marked "implementation-specific" are explained in the individual content type descriptions below.

**string Markup**    
_Implementation-specific._ The actual Base Markup to be output. 

**string InlineStyles**   
Raw CSS to be output to the page on which the content is placed. This will be added via `ClientResources.RequireStyleInline()`. Available in Edit Mode.

**string InlineScript**   
Raw Javascript to be output to the page on which the content is placed. This will be added via `ClientResources.RequireScriptInline()`. Available in Edit Mode.

**string StylesheetReferences**    
URLs of stylesheets to be referenced via a `LINK` tag (one URL per line). These will be added via to `ClientResources.RequireStyle()`. Available in Edit Mode.

**string ScriptReferences**    
URLs of script files to be referenced via a `SCRIPT` tag (one URL per line). These will be added via `ClientResources.RequireScript().AtFooter()`. Available in Edit Mode.

**string GetTextOfResource(string token)**  
_Implementation-specific._ Gets the text of the resource referenced by the filename. In most all cases, this is just a wrapper for `GetBytesOfResource()`.

**string GetTextOfResource(string token)**  
_Implementation-specific._ Gets an array of bytes for the resource referenced by the filename.

**IEnumerable<string> GetResources()**   
_Implementation-specific._ Gets a list of available resource names.

### BlockData: Markup Block

* Base Markup is entered to a `Markup` property directly in Edit Mode.
* Local Resources are files in the assets folder for the block. Anything with `.js` or `.css` extensions will be referenced.

### MediaData: Markup File

Mapped to the `.html` and `.htm` extensions by default. Represents a single HTML file.

* Base Markup is the content of the file itself.
* Local Resources are files in _the same asset folder as the Markup File_, and will be evaluated and referenced as with Markup Block. (Clearly, isolate your Markup File in its own asset folder, unless more than one file should share the same local resources).

### MediaData: Markup Archive File

Mapped to the `.app` extension by default. Represents a zip archive with its extension changed. Related markup assets can be placed in this archive so they can be managed as a single file.

* Base Markup is an HTML file in the _root_ of the archive with the same base name as the archive and a `.html` extension. (e.g. -- if the archive is called `markup.app` then the file in the root should be `markup.html`)
* Local Resources are files in the _root_ of the archive, and will be evaluated and referenced as with Markup Block.

Note the emphasis on _root_. This means you can't zip a directory because those files will be _in the directory_ in the root of the archive. On Windows, this means you highlight a group of files (_not_ their directory) and select `Send To...Compressed (zipped) folder`.

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