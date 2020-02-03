const defaultDatabases = ["admin", "config", "local"]

// Create mongo connection
let mongo = new Mongo();

// Remove old databases
let db = mongo.getDB("test");
let oldDbs = db.adminCommand('listDatabases').databases;
for (let i = 0; i < oldDbs.length; i++) {
    // Make sure we don't delete default databases
    if (!defaultDatabases.includes(oldDbs[i].name)) {
        mongo.getDB(oldDbs[i].name).dropDatabase();
    }
}

// Insert test records
db = mongo.getDB("test");
db.tset.insert({ "name": "Hemlo World!", "value": "This is pretty neat-o!" });

// Create user and role databases
db = mongo.getDB("users");
db.users.insert({
    "Username": "JohnnyBoi@net.net",
    "NormalizedUsername": "JOHNNYBOI@NET.NET",
    "Email": "JOHNNYBOI@NET.NET",
    "NormalizedEmail": "JOHNNYBOI@NET.NET",
    "EmailConfirmed": false,
    "FullName": "Johnny Boi",
//  "Password": "aBc123$%^",
    "PasswordHash": "AQAAAAEAACcQAAAAECf1ja3G75KH70FUM4K+Y4YlQjK21hkvG/p2dGQMjUy1lPWeR8/o2QTY9bPoD3ZAow==",
    "SecurityStamp": "A3HM2WLAG7ORJWWX6ELVL5CVXEIGWNY6",
    "FailedLoginAttempts": 0,
    "LockoutEnabled": true,
    "LockedOutUntil": null
})

let johnnyBoiId = db.users.find({ "NormalizedUsername": "JOHNNYBOI@NET.NET" })[0]._id;

db.roles.insert({
    "Name": "Tree Planters",
    "NormalizedName": "TREE PLANTERS",
    "Users": [johnnyBoiId]
})

db = mongo.getDB("assets");
db.types.insertMany([{
        "Name": "Laptop",
        "NormalizedName": "LAPTOP",
        "Description": "Laptop computer"
    },
    {
        "Name": "Laptop A/C Adapter",
        "NormalizedName": "LAPTOP A/C ADAPTER",
        "Description": "Laptop charger"
    }]);

db.manufacturers.insertMany([{
        "Name": "Lenovo",
        "EmailAddresses": ["support@lenovo.com"],
        "PhoneNumbers": []
    },
    {
        "Name": "Dell",
        "EmailAddresses": [],
        "PhoneNumbers": []
    }]);

let laptop = db.types.find({ "NormalizedName": "LAPTOP" })[0]._id;
let lenovo = db.manufacturers.find({ "Name": "Lenovo" })[0]._id;
let dell = db.manufacturers.find({ "Name": "Dell" })[0]._id;

db.models.insertMany([{
    "Name": "Lenovo P1 gen 2",
    "NormalizedName": "LENOVO P1 GEN 2",
    "Type": laptop,
    "ModelNumber": "20QU",
    "Manufacturer": lenovo
},
{
    "Name": "Dell Precision 5520",
    "NormalizedName": "DELL PRECISION 5520",
    "Type": laptop,
    "ModelNumber": "M5520",
    "Manufacturer": dell
}]);