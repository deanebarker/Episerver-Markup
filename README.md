# Episerver Markup

A library to assist working with hand-coded markup in Episerver CMS. The goal is to allow front-end developers to deploy HTML/CSS/JS code as content, and have that code "bootstrap" itself by managing all necessary references to supporting JS and CSS files.

## Summary

It provides three content types:

* Markup Block
* Markup File
* Markup Archive File

Content based on these types represents hand-coded markup and required style and script resources (hereafter generically referred to as "resources").

This content can be dragged into a content area where it will output to the page (the "Base Markup") and output CSS and/or JavaScript in `STYLE` or `SCRIPT` tags ("Inline Resources"), or output `LINK` or `SCRIPT` tags to reference off-page styles and/or scripts, which can be served from the Episerver site itself ("Local Resources"), or located elsewhere ("Remote Resources").

Markup can be either manually added in Edit Mode (through a `MarkupBlock`) or added via file upload (`MarkupFile` or `MarkupArchiveFile`).

>**Note:** this code requires project references to `Episerver.dll`, `Episerver.Data.dll`, `Episerver.Shell`, and `Episerver.Framework.dll`. This code has been successfully compiled against v10.10.3 of those assemblies.

## The Four Elements of Markup

Adding raw markup to a page can logically result in the output of four things:

1. **Base Markup.** This is the markup that is output to the page where the content is placed -- usually HTML, but can be any text.
2. **Inline Resources (Script or Style).** This is inline Javascript and CSS that is directly output to the page.
3. **Local Resources (Script or Style).** These are Javascript and CSS files managed in the Episerver site and associated the content (through various methods, described below) that are referenced through `LINK` and `SCRIPT` tags.
4. **Remote Resource URLs (Script or Style).** These are URLs to off-site Javascript and CSS that are referenced through `LINK` and `SCRIPT` tags.

Base Markup will always be output wherever the content is placed. The other three elements may or may not be output, depending on need.

>**Example:**    
An SVG graphic requires nothing but a snippet of XML (the Base Markup).

>**Example:**    
A client-app written in Vue.js requires a snippet of HTML (the Base Markup), a line of JavaScript to activate the application on page load (an Inline Script Resource), the remote Vue.js client library (a Remote Script Resource), and a stylesheet written for this specific application (a Local Resource).

Base Markup and Inline Resources are always output to the page. Local and Remote Resources are evaluated for reference inclusion.

(To clarify: a _resource_ is the content itself, a _reference_ is the tag that refers to that content. So, a managed CSS file is a _resource_. The `LINK` tag that loads that file into a page is a _reference_.)

## Content Types

There are three content types -- a block, and two media items.

Of the four elements listed above, Inline Resources and Remote Resources are handled the same in all three content types:

* Inline Resources are directly entered in two properties: `InlineStyles` and `InlineScript`.
* Remote Resource URLs are directly entered in two properties: `StylesheetReferences` and `ScriptReferences`, one URL per line.

Each content type handles sources its Base Markup and Local Resources through implementation-specific methods:

### BlockData: Markup Block

* Base Markup is entered in a `Markup` property directly in Edit Mode.
* Local Resources are files in the assets folder for the block. Anything with `.js` or `.css` extensions will be referenced.

### MediaData: Markup File

Mapped to the `.html` and `.htm` extensions by default. Represents a single HTML file.

* Base Markup is the content of the file itself.
* Local Resources are files in _the same asset folder as the Markup File_, and will be evaluated and referenced as with Markup Block. (Clearly, isolate your Markup File in its own asset folder, unless more than one file should share the same Local Resources).

### MediaData: Markup Archive File

Mapped to the `.app` extension by default. Represents a zip archive with its extension changed. Related markup assets can be placed in this archive so they can be managed as a single file.

* Base Markup is an HTML file in the _root_ of the archive with the same base name as the archive and a `.html` extension. (e.g. -- if the archive is called `markup.app` then the file in the root should be `markup.html`)
* Local Resources are files in the _root_ of the archive, and will be evaluated and referenced as with Markup Block.

Note the emphasis on _root_. This means you can't zip a directory because those files will be _in the directory_ in the root of the archive. On Windows, this means you highlight a group of files (_not_ their directory) and select `Send To...Compressed (zipped) folder`.

A video of the Markup Archive File in action is available here:

https://vimeo.com/236467181

## Poor Man's Code Editor

All properties available in Edit Mode use an instance of the "Poor Man's Code Editor" for their UI editing component. This provides a simple textarea with several script-enabled behaviors optimized for code editing.

https://gist.github.com/deanebarker/f1c2542b3a510eb992c76c7e07c2f16b

## Resource and Reference Placement

References to Resources are placed using the `ClientResources` helper class.

* Inline Resources use `ClientResources.RequireStyleInline()` and `ClientResources.RequireScriptInline()`
* Local and Remote Resources use `ClientResources.RequireStyle()` and `ClientResources.RequireScript().AtFooter()`

By default, the Alloy demo site places these references --

* Styles: in the `HEAD` tag
* Scripts: just above the closing `BODY` tag

Your templating may vary. See the `OnBeforeAddReference` event below for how to override reference placement.

Local Resources will be routed through a resource handler class. By default, this path is:

    /markup.resource?id=[ID of the content]&file=[name of the file]

The resource handler reads the specified content object and extracts and returns the resource contents with the correct MIME type. This handler architecture is used for two reasons:

1. The Markup Archive File is a zip archive of multiple filee. The handler opens the archive and extracts the resource (see "Markup Archive File" below). There is technically no direct URL to this resource.
2. Resource output is wrapped by an event, to allow custom processing before delivery.

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
