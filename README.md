# Package Tracker

- Track DHL Packages
- Automatically send message when package was delivered.

## REST API

### Start Automatic Update Process

Send a Patch request.

```url
http://localhost:5000/package/startupdating
```

### Add a new package

In order to track a new package, send a post request. Provide the ``trackingNo`` of your package, as well as a ``productDescription`` which describes the content of your package.

```url
http://localhost:5000/package?packageTrackingNo=<trackingNo>&productDescription=<descriptionOfProduct
```

Example:

```url
http://localhost:5000/package?packageTrackingNo=00329907888758&productDescription=shoes
```

The server stores your new package and returns it.

### Get a package

To get an existing package, send a Get request to:

```url
http://localhost:5000/package?packageTrackingNo=<trackingNo>
```
