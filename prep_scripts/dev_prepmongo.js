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
    "Username": "JohnnyBoi",
    "Email": "jboy@appleseed.net",
    "EmailConfirmed": false,
    "PasswordHash": null,
    "SecurityStamp": "",
    "FailedLoginAttempts": 0,
    "LockedOut": false,
    "LockedOutUntil" : null
})

db.roles.insert({
    "Name": "treePlanters",
    "DisplayName": "Tree Planters",
    "Users": []
})
