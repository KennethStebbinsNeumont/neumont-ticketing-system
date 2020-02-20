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
db.users.insertMany([{
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
},
{
	"Username": "LBelcher@BobsBurgers.com",
	"NormalizedUsername": "LBELCHER@BOBSBURGERS.COM",
	"Email": "LBELCHER@BOBSBURGERS.COM",
	"NormalizedEmail": "LBELCHER@BOBSBURGERS.COM",
	"EmailConfirmed": false,
	"FullName": "Louise Belcher",
	//  "Password": "aBc123$%^",
	"PasswordHash": "AQAAAAEAACcQAAAAECf1ja3G75KH70FUM4K+Y4YlQjK21hkvG/p2dGQMjUy1lPWeR8/o2QTY9bPoD3ZAow==",
	"SecurityStamp": null,
	"FailedLoginAttempts": 0,
	"LockoutEnabled": true,
	"LockedOutUntil": null
}])

let johnnyBoiId = db.users.find({ "NormalizedUsername": "JOHNNYBOI@NET.NET" })[0]._id;
let lBelcherId = db.users.find({ "NormalizedUsername": "LBELCHER@BOBSBURGERS.COM" })[0]._id;

db.roles.insertMany([{
	"Name": "Technicians",
	"NormalizedName": "TECHNICIANS",
	"Users": [johnnyBoiId]
},
{
	"Name": "Administrators",
	"NormalizedName": "ADMINISTRATORS",
	"Users": [lBelcherId]
}]);

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
        "NormalizedName": "LENOVO",
        "EmailAddresses": ["support@lenovo.com"],
        "PhoneNumbers": []
    },
    {
        "Name": "Dell",
        "NormalizedName":"DELL",
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
let p1g2 = db.models.find({ "Name": "Lenovo P1 gen 2" })[0]._id;
let p5520 = db.models.find({ "Name": "Dell Precision 5520" })[0]._id;

db = mongo.getDB("owners");
db.owners.insertMany([{
    "Name": "Michael Johns",
    "NormalizedName": "MICHAELJOHNS",
    "EmailAddresses": [
        "mjohns@student.neumont.edu"
    ],
    "PhoneNumbers": [
        "4204206969"
    ],
    "PreferredName": {
        "First": "Mikey",
        "NormalizedFirst": "MIKEY",
        "Last": "Johns",
        "NormalizedLast": "JOHNS"
    }
},
{
    "Name": "Mikey Jews",
    "NormalizedName": "MIKEYJEWS",
    "EmailAddresses": [
        "mjews@student.neumont.edu"
    ],
    "PhoneNumbers": [
        "9996662222"
    ],
},
{
    "Name": "Michael Jones",
    "NormalizedName": "MICHAELJONES",
    "EmailAddresses": [
        "mjones@student.neumont.edu"
    ],
    "PhoneNumbers": [
        "9990002222"
    ],
    "PreferredName": {
        "First": "Mikey",
        "NormalizedFirst": "MIKEY",
        "Middle": "\"Jersey\"",
        "NormalizedMiddle": "JERSEY",
        "Last": "Jones",
        "NormalizedLast": "JONES"
    }
},
{
    "Name": "John Cena",
    "NormalizedName": "JOHNCENA",
    "EmailAddresses": [
        "jcena@student.neumont.edu"
    ],
    "PhoneNumbers": [
        "8013022860"
    ],
    "PreferredName": {
        "First": "Gerald",
        "NormalizedFirst": "GERALD",
        "Middle": "\"The Stone\"",
        "NormalizedMiddle": "THESTONE",
        "Last": "Phoenix",
        "NormalizedLast": "PHOENIX"
    }
}]);
let mjones1 = db.owners.find({"NormalizedName": "MICHAELJOHNS" })[0]._id;
let mjones2 = db.owners.find({ "NormalizedName": "MIKEYJEWS" })[0]._id;
let mjones3 = db.owners.find({ "NormalizedName": "MICHAELJONES" })[0]._id;
let jcena = db.owners.find({ "NormalizedName": "JOHNCENA" })[0]._id;

db = mongo.getDB("assets");
db.assets.insertMany([{
    "SerialNumber": "QR30FH2",
    "NormalizedSerialNumber": "QR30FH2",
    "Model": p5520,
    "Owner": mjones1
},
{
    "SerialNumber": "QR30FH3",
    "NormalizedSerialNumber": "QR30FH3",
    "Model": p5520,
    "Owner": mjones2
},
{
    "SerialNumber": "QR30FH2",
    "NormalizedSerialNumber": "QR30FH2",
    "Model": p5520,
    "Owner": mjones3
},
{
    "SerialNumber": "Johns",
    "NormalizedSerialNumber": "JOHNS",
    "Model": p1g2,
    "Owner": jcena
    }]);

db = mongo.getDB("tickets")

db.repairs.insertMany([{
		"Name": "Dell depot repair",
		"NormalizedName": "DELLDEPOTREPAIR",
		"Description": "Send a Dell machine to Dell's Advanced Resolution Center to repair.",
		"AppliesTo": {
			"Types": [
				laptop
			],
			"Manufacturers": [
				dell
			],
			"Models": []
		},
		"AdditionalFields": [
			"Helped by",
			"Service request #"
		],
		"Steps": [
			{
				"Name": "Receive machine from student",
				"NormalizedName": "RECEIVEMACHINEFROMSTUDENT",
				"Description": "",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			},
			{
				"Name": "Incoming benchmark",
				"NormalizedName": "INCOMINGBENCHMARK",
				"Description": "Run a battery of tests against the machine to confirm the described problem(s) and discover new ones.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": [
					{
						"Name": "POST/Boot test",
						"NormalizedName": "POSTBOOTTEST",
						"Description": "Boot the machine into an appropriate benching image.",
						"StartedDate": ISODate("0001-01-01T00:00:00Z"),
						"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
						"Comments": null,
						"SubSteps": []
					},
					{
						"Name": "Performance tests",
						"NormalizedName": "PERFORMANCETESTS",
						"Description": "Push the machine to its limits",
						"StartedDate": ISODate("0001-01-01T00:00:00Z"),
						"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
						"Comments": null,
						"SubSteps": [
							{
								"Name": "Intel Processor Diagnostic Tool",
								"NormalizedName": "INTELPROCESSORDIAGNOSTICTOOL",
								"Description": "A test made by Intel to discover problems within the CPU.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Cinebench",
								"NormalizedName": "CINEBENCH",
								"Description": "A CPU benchmark that renders a 3D scene as fast as the machine can.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "FurMark (dGPU)",
								"NormalizedName": "FURMARKDGPU",
								"Description": "A simple GPU test that runs on the dedicated graphics chip in the machine.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "FurMark (iGPU)",
								"NormalizedName": "FURMARKIGPU",
								"Description": "A simple GPU test that runs on the machine's processor's integrated graphics processor.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Heaven Benchmark",
								"NormalizedName": "HEAVENBENCHMARK",
								"Description": "A gaming benchmark that hits both the CPU and GPU hard.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							}
						]
					},
					{
						"Name": "Stability Tests",
						"NormalizedName": "STABILITYTESTS",
						"Description": "These tests determine how stable the machine is under the largest of loads.",
						"StartedDate": ISODate("0001-01-01T00:00:00Z"),
						"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
						"Comments": null,
						"SubSteps": [
							{
								"Name": "Prime95",
								"NormalizedName": "PRIME95",
								"Description": "A CPU torture-test designed to put as much load on and generate the most heat from the CPU.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "MemTest64",
								"NormalizedName": "MEMTEST64",
								"Description": "A test that runs operations on the machine's memory, stressing as much of it as possible.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							}
						]
					},
					{
						"Name": "Basic tests",
						"NormalizedName": "BASICTESTS",
						"Description": "These tests ensure that the basic functions of the machine are in working order.",
						"StartedDate": ISODate("0001-01-01T00:00:00Z"),
						"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
						"Comments": null,
						"SubSteps": [
							{
								"Name": "USB-A test",
								"NormalizedName": "USBATEST",
								"Description": "Connect a flash drive to each USB-A port and ensure the machine can read it",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "USB-C test",
								"NormalizedName": "USBCTEST",
								"Description": "Connect a flash drive to each USB-C port and ensure the machine can read it",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Speaker test",
								"NormalizedName": "SPEAKERTEST",
								"Description": "Play a sound through the computer's speakers and ensure there is no audible distortion and that the speakers are balanced.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Headphone test",
								"NormalizedName": "HEADPHONETEST",
								"Description": "Connect headphones to the machine and ensure both audio channels sound balanced and clear.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Wi-Fi test",
								"NormalizedName": "WIFITEST",
								"Description": "Connect the machine to Wi-Fi and load a webpage.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "HDMI test",
								"NormalizedName": "HDMITEST",
								"Description": "Connect the machine to an HDMI monitor and make sure an image appears.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "SD card test",
								"NormalizedName": "SDCARDTEST",
								"Description": "Connect an SD card to the machine and ensure the machine can read it",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Keyboard test",
								"NormalizedName": "KEYBOARDTEST",
								"Description": "Go to https://keyboardtester.com/tester.html and ensure that every key on the keyboard registers reliably and accurately.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Trackpad test",
								"NormalizedName": "TRACKPADTEST",
								"Description": "Move the cursor up, down, left, and right using the trackpad. Ensure both left and right clicks register. Ensure there are no errant clicks registered when moving a finger from the bottom to the top of the trackpad.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Monitor test",
								"NormalizedName": "MONITORTEST",
								"Description": "Run Monitor Test and ensure that no clusters of dead pixels nor light spots are seen.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							},
							{
								"Name": "Touchscreen test",
								"NormalizedName": "TOUCHSCREENTEST",
								"Description": "Run Monitor Test and ensure that all of the touchscreen tests pass. Tap on the display with 5 fingers and confirm that Windows registers all 5 taps.",
								"StartedDate": ISODate("0001-01-01T00:00:00Z"),
								"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
								"Comments": null,
								"SubSteps": []
							}
						]
					}
				]
			},
			{
				"Name": "Create Dell support ticket",
				"NormalizedName": "CREATEDELLSUPPORTTICKET",
				"Description": "Call Dell support and create a service request to send this machine to ARC.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			},
			{
				"Name": "Ship the machine",
				"NormalizedName": "SHIPTHEMACHINE",
				"Description": "Pack and ship the machine to ARC.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			},
			{
				"Name": "Receive the machine",
				"NormalizedName": "RECEIVETHEMACHINE",
				"Description": "Unpack the machine and prepare it for testing.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			},
			{
				"Name": "Outgoing benchmark",
				"NormalizedName": "OUTGOINGBENCHMARK",
				"Description": "Perform the same tests as the incoming benchmark to ensure the original problems have been fixed and that no new problems are present.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			},
			{
				"Name": "Contact student",
				"NormalizedName": "CONTACTSTUDENT",
				"Description": "Contact the student to pick up their laptop.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			},
			{
				"Name": "Return to student",
				"NormalizedName": "RETURNTOSTUDENT",
				"Description": "Return the machine to its owner.",
				"StartedDate": ISODate("0001-01-01T00:00:00Z"),
				"CompletedDate": ISODate("0001-01-01T00:00:00Z"),
				"Comments": null,
				"SubSteps": []
			}
		]
}]);