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
    "EmailConfirmed": false,
    "FullName": "Johnny Boi",
//  "Password": "aBc123$%^",
    "PasswordHash": "AQAAAAEAACcQAAAAECf1ja3G75KH70FUM4K+Y4YlQjK21hkvG/p2dGQMjUy1lPWeR8/o2QTY9bPoD3ZAow==",
    "SecurityStamp": "A3HM2WLAG7ORJWWX6ELVL5CVXEIGWNY6",
    "FailedLoginAttempts": 0,
    "LockoutEnabled": true,
    "LockedOutUntil": null
})

var johnnyBoiId = db.users.find({ "Username": "JOHNNYBOI@NET.NET" })[0]._id;

db.roles.insert({
    "Name": "treePlanters",
    "DisplayName": "Tree Planters",
    "Users": [johnnyBoiId]
})
