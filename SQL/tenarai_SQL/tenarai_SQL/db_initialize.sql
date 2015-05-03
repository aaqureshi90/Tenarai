/***********************************************************
 *      Insert any default data through this script        *
 *      This script can be run first thing after creating  *
 *      a new database.                                    *
 **********************************************************/

INSERT INTO tblEmailType (emt_CODE, emt_DESCRIPTION)
VALUES ('P', 'Primary Email Address')

INSERT INTO tblEmailType (emt_CODE, emt_DESCRIPTION)
VALUES ('S', 'Secondary Email Address')

INSERT INTO tblPhoneType (pht_CODE, pht_DESCRIPTION)
VALUES ('H', 'Home Phone Number')

INSERT INTO tblPhoneType (pht_CODE, pht_DESCRIPTION)
VALUES ('W', 'Work Phone Number')
