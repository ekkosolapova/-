create table if not exists Doctors
(
    DoctorId                    int                not null,
    DoctorName                  varchar(50)        not null,
    DoctorSpecialization        varchar(100)       not null,
    DoctorExperience            interval           not null,
    constraint Doctors_PK primary key (DoctorId)
);

create table if not exists Clinics
(
    ClinicId          int              not null,
    ClinicAddress     varchar(256)     not null,
    ClinicPhoneNumber varchar(12)      not null,
    ClinicRating      real             not null,
    constraint Clinics_PK primary key (ClinicId),
    constraint Clinics_K1 unique (ClinicAddress),
    constraint Clinics_K2 unique (ClinicPhoneNumber)
);

create table if not exists Patients
(
    PatientId               int             not null,
    PatientFullName         varchar(50)     not null,
    PatientBirthday         timestamp       not null,
    PatientPhoneNumber      varchar(12)     not null,
    MainClinicId          int             not null,
    constraint Patients_PK primary key (PatientId),
    constraint Patients_MainClinicId_FK1 foreign key (MainClinicId) references Clinics (ClinicId)
);

create table if not exists Visits
(
    VisitId             int             not null,
    ClinicId            int             not null,
    PatientId           int             not null,
    DoctorId            int             not null,
    VisitTimestamp      timestamp       not null,
    VisitDuration       interval        not null,
    VisitComment        text            not null,
    constraint Visits_PK primary key (VisitId),
    constraint Visits_K1 unique (VisitTimestamp, PatientId),
    constraint Visits_K2 unique (VisitTimestamp, DoctorId),
    constraint Visits_ClinicId_FK1 foreign key (ClinicId) references Clinics (ClinicId),
    constraint Visits_PatientId_FK2 foreign key (PatientId) references Patients (PatientId),
    constraint Visits_DoctorId_FK3 foreign key (DoctorId) references Doctors (DoctorId)

);

create table if not exists Drugs
(
    DrugName    varchar(50)     not null,
    DrugPrice   real            not null,
    constraint Drugs_PK primary key (DrugName)
);

create table if not exists Prescription
(
    ValidityPeriod          interval         not null,
    StartTimestamp          timestamp        not null,
    DrugName                varchar(50)      not null,
    VisitId                 int              not null,
    constraint Prescription_PK primary key (DrugName, VisitId),
    constraint Prescription_VisitId_FK1 foreign key (VisitId) references Visits (VisitId),
    constraint Prescription_DrugName_FK2 foreign key (DrugName) references Drugs (DrugName)
);
