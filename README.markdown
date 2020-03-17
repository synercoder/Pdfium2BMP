# PdfiumViewer

Apache 2.0 License.

## Using the library

The PdfiumViewer requires native PDFium libraries. These are not included
in the PdfiumViewer NuGet package. See the [Installation instructions](https://github.com/pvginkel/PdfiumViewer/wiki/Installation-instructions)
Wiki page for more information on how to add these.

## Building PDFium

Instructions to build the PDFium library can be found on the [Building PDFium](https://github.com/pvginkel/PdfiumViewer/wiki/Building-PDFium)
wiki page. However, if you are just looking to use the PdfiumViewer component
or looking for a compiled version of PDFium, these steps are not required.
NuGet packages with precompiled PDFium libraries are made available for
usage with PdfiumViewer. See the chapter on **Using the library** for more
information.

Alternatively, the [PdfiumBuild](https://github.com/pvginkel/PdfiumBuild) project
is provided to automate building PDFium. This project contains scripts to
build PdfiumViewer specific versions of the PDFium library. This project
is configured on a build server to compile PDFium daily. Please refer to
the [PdfiumBuild](https://github.com/pvginkel/PdfiumBuild) project page
for the location of the output of the build server. The PdfiumViewer specific
libraries are located in the `PdfiumViewer-...` target directories.

## License

PdfiumViewer is licensed under the Apache 2.0 license. See the license details for how PDFium is licensed.
