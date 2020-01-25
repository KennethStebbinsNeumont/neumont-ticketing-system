Hello, world!

This directory is for scripts used to prepare environments before deployment.

run.sh is called by the pipeline, and depending on which pipeline is calling it, will
either run every script that starts with prod_ or dev_ .

You can put anything you want in here, but the pipeline won't care unless it fits the
criteria above.
