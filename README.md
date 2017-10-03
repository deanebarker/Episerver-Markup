# Episerver Markup

This a library to assist working with "hand-coded" markup files in Episerver CMS.

It provides two media types:

* Markup File
* Markup Archive File

The intended usage is that hand-coded markup files (usually HTML) can be uploaded into the Episerver workarea, and the resulting media item can be dragged into a content area, where it will render its contents.

>Note: this code will not compile without adding local references to `Episerver.dll`, `Episerver.Data.dll`, and `Episerver.Framework.dll`. This code has been compiled against v10.10.3 of those assemblies.

## Markup File

Mapped to the `.html` and `.htm` extensions by default.

Represents an HTML file. When placed on a page, it will write its contents to the response.

If delimiter comments are in the file (`<!-- start -->` and `<!-- end -->` by default), it will only render the content _between_ those comments. Anything above and below will be discard. (Use when you might need a full HTML document for local development, but simply want to extract a section for publication.)

All files in the same asset folder as the Markup File will be evaluated. Anything with a `.js` extension will be passed to to `ClientResources.RequireScript().AtFooter()` and anything with a `.css` extension will be passed to `ClientResources.RequireStyle()`. These will be direct URLs links to those assets.

(In the Alloy demo site, those are rendered in the `HEAD` tag and just before the closing `BODY` tag, but your templating may vary.)

Properties exist for "Required Script URLs" and "Required Stylesheet URLs." Those are handled just like local JS and CSS, and are used to bring in other resources (an external JavaScript library, for instance).

## Markup Archive File

Mapped to the `.app` extension by default.

Represents a zip archive with its extension changed. Related markup assets can be placed in this archive so they can be managed as a single file.

Any HTML, JS, and CSS files in the _root_ of this archive will be handled as above. (Instead of looking in the file's assets folder, it searches the zip archive.)

The base HTML file should have the same name as the Markup Archive File (if the archive is called `mymarkup.app` then the file in the root should be `mymarkup.html`). And remember that the files need to be in the _root_ of the archive, which means you can't zip a folder containing the files -- rather, you need to zip the files as a group.

References to JS and CSS are added with a URL pattern that routes requests to a custom handler (`markup.resources` by default) which extracts the file contents from the zip.

A video of the Markup Archive File in action is available here:

https://vimeo.com/236467181

>Deane Barker    
deane@blendinteractive.com    
@gadgetopia