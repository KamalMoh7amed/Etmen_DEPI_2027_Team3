# Etmen DEPI Project — Implementation Task Board

> **Context:** All interfaces, DTOs, entities, DAL configurations, migrations, and the UoW are already done.
> Every stub file throws `NotImplementedException` and has a `// TODO` comment explaining exactly what to write.
> **Do not touch the front-end (Views) for now — focus on back-end implementation only.**

---

## 🗂️ How to work

1. Pull the latest code from the repo.
2. Find your tasks below, open the stub file, read the `// TODO` comments carefully.
3. Replace each `throw new NotImplementedException();` with real code.
4. Build and run — the project should compile as-is (stubs throw at runtime, not compile time).
5. Write at least one manual test or unit test for each method before pushing.
6. Open a PR per task group and tag your reviewer.

---

## 👤 Member 1 — Auth & Patient Profile

**Services (BLL):**

| File | Methods to implement |
|------|----------------------|
| `Etmen_BLL/Repositories/Services/AuthService.cs` | `RegisterAsync`, `LoginAsync`, `VerifyEmailAsync`, `ForgotPasswordAsync`, `ResetPasswordAsync`, `DeactivateAccountAsync`, `IsEmailTakenAsync` |
| `Etmen_BLL/Repositories/Services/PatientService.cs` | `GetProfileAsync`, `UpdateProfileAsync`, `GetDashboardAsync` |

**Controller (PL):**

| File | Actions to implement |
|------|----------------------|
| `Etmen_PL/Controllers/AccountController.cs` | All actions: `Register`, `Login`, `Logout`, `VerifyEmail`, `ForgotPassword`, `ResetPassword` |
| `Etmen_PL/Controllers/PatientController.cs` | `Dashboard`, `Profile` (GET + POST) |

**DAL Repositories:**

| File | All methods |
|------|-------------|
| `Etmen_DAL/Repositories/Implementations/PatientProfileRepository.cs` | All 7 methods |

**Notes:**
- Use `UserManager<ApplicationUser>` and `SignInManager` injected via DI for auth operations.
- JWT token generation goes in `AuthService.LoginAsync` — store signing key in `appsettings.json`.
- `GetDashboardAsync` should aggregate data from at least: upcoming appointments, latest risk, unread alerts.

---

## 👤 Member 2 — Doctor & Appointments

**Services (BLL):**

| File | Methods to implement |
|------|----------------------|
| `Etmen_BLL/Repositories/Services/DoctorService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/AppointmentService.cs` | All methods |

**Controller (PL):**

| File | Actions to implement |
|------|----------------------|
| `Etmen_PL/Controllers/DoctorController.cs` | All actions |
| `Etmen_PL/Controllers/PatientController.cs` | `Appointments`, `CancelAppointment` |

**DAL Repositories:**

| File | All methods |
|------|-------------|
| `Etmen_DAL/Repositories/Implementations/DoctorProfileRepository.cs` | All 6 methods |
| `Etmen_DAL/Repositories/Implementations/AppointmentRepository.cs` | All 8 methods |
| `Etmen_DAL/Repositories/Implementations/AvailableSlotRepository.cs` | All 6 methods |

**Notes:**
- `BookAppointmentAsync` must be transactional: check slot availability, create Appointment, mark slot booked — wrap in `BeginTransactionAsync/CommitTransactionAsync`.
- `BulkAddSlotsAsync` in DoctorService should generate slots from a start/end date range + time intervals.
- Use `[Authorize(Roles = "Doctor")]` on all DoctorController actions.

---

## 👤 Member 3 — Medical Records, Lab Results & Risk Engine

**Services (BLL):**

| File | Methods to implement |
|------|----------------------|
| `Etmen_BLL/Repositories/Services/MedicalRecordService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/LabService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/RiskService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/PatientService.cs` | `GetMedicalRecordsAsync`, `GetLatestMedicalRecordAsync`, `AddMedicalRecordAsync`, `AssessRiskAsync`, `GetLatestRiskAssessmentAsync`, `GetRiskHistoryAsync` |

**Controller (PL):**

| File | Actions to implement |
|------|----------------------|
| `Etmen_PL/Controllers/PatientController.cs` | `MedicalRecords`, `AddMedicalRecord`, `RiskAssessment` (GET+POST), `RiskResult`, `LabResults` |

**DAL Repositories:**

| File | All methods |
|------|-------------|
| `Etmen_DAL/Repositories/Implementations/MedicalRecordRepository.cs` | All 5 methods |
| `Etmen_DAL/Repositories/Implementations/LabResultRepository.cs` | All 7 methods |
| `Etmen_DAL/Repositories/Implementations/RiskAssessmentRepository.cs` | All 7 methods |

**Notes:**
- `RiskCalculatorHelper` (already in `Etmen_BLL/Helpers/`) has the scoring logic — use it in `RiskService.CalculateRiskAsync`.
- `BmiHelper` is also ready — use it when creating medical records to auto-compute BMI.
- Lab file upload: store the file in `wwwroot/uploads/labs/` and save the path in the entity; OCR is optional/bonus.

---

## 👤 Member 4 — Crisis, Emergency & Nearby

**Services (BLL):**

| File | Methods to implement |
|------|----------------------|
| `Etmen_BLL/Repositories/Services/CrisisService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/CrisisRiskEngineService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/EmergencyService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/NearbyService.cs` | All methods |

**Controller (PL):**

| File | Actions to implement |
|------|----------------------|
| `Etmen_PL/Controllers/PatientController.cs` | `Nearby` |

**DAL Repositories:**

| File | All methods |
|------|-------------|
| `Etmen_DAL/Repositories/Implementations/CrisisConfigurationRepository.cs` | All 9 methods |
| `Etmen_DAL/Repositories/Implementations/OutbreakZoneRepository.cs` | All 6 methods |
| `Etmen_DAL/Repositories/Implementations/EmergencyRequestRepository.cs` | All 10 methods |
| `Etmen_DAL/Repositories/Implementations/HealthcareProviderRepository.cs` | All 6 methods |

**Notes:**
- `GeoHelper` in `Etmen_DAL/Helpers/GeoHelper.cs` already provides Haversine distance — use it in nearby/outbreak zone queries.
- `CrisisRiskEngineService.CalculateCrisisRiskAsync` must load SymptomWeights from the active crisis and score the patient's reported symptoms.
- Only one crisis can be active at a time — enforce this in `ActivateCrisisAsync`.
- `EmergencyService.CreateEmergencyRequestAsync` should auto-find the nearest available provider.

---

## 👤 Member 5 — Alerts, Notifications, Family & AI Chat + Admin

**Services (BLL):**

| File | Methods to implement |
|------|----------------------|
| `Etmen_BLL/Repositories/Services/AlertService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/NotificationService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/FamilyService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/AIChatService.cs` | All methods |
| `Etmen_BLL/Repositories/Services/AdminService.cs` | All methods |

**Controller (PL):**

| File | Actions to implement |
|------|----------------------|
| `Etmen_PL/Controllers/ChatController.cs` | `Index`, `Send`, `ClearHistory` |
| `Etmen_PL/Controllers/PatientController.cs` | `Alerts` |

**DAL Repositories:**

| File | All methods |
|------|-------------|
| `Etmen_DAL/Repositories/Implementations/AlertRepository.cs` | All 8 methods |
| `Etmen_DAL/Repositories/Implementations/NotificationRepository.cs` | All 7 methods |
| `Etmen_DAL/Repositories/Implementations/FamilyLinkRepository.cs` | All 7 methods |
| `Etmen_DAL/Repositories/Implementations/ChatMessageRepository.cs` | All 6 methods |

**Notes:**
- `AIChatService.SendMessageAsync` calls an external LLM API (configure base URL + key in `appsettings.json`); include the user's last 10 messages as conversation context.
- `FamilyService.InviteFamilyMemberAsync` should generate a `Guid.NewGuid().ToString()` token, save it, and send it via `INotificationService`.
- Admin actions (`AdminService`) require `[Authorize(Roles = "Admin")]` — add an `AdminController` or extend `HomeController` as needed.
- `NotificationService.ClearExpiredNotificationsAsync` can be triggered by a background job (e.g., `IHostedService`) — wire it up in `Program.cs`.

---

## 🔧 Shared setup checklist (all members)

- [ ] Register your service + its interface in `Program.cs` with the correct lifetime (`AddScoped` for services and repositories).
- [ ] Make sure AutoMapper profiles in `Etmen_BLL/Mapping/BLLMappingProfile.cs` include mappings for your DTOs — add any missing ones.
- [ ] Do **not** change interfaces, DTOs, or entity classes — only implement the stubs.
- [ ] Run `dotnet build` before every push to catch compile errors early.

---

## 📋 Quick reference — project structure

```
Etmen_Domain/          → Entities & Enums (READ ONLY)
Etmen_DAL/
  Configurations/      → EF configs (READ ONLY)
  DbContext/           → EtmenDbContext (READ ONLY)
  Migrations/          → Migrations (READ ONLY)
  Repositories/
    Interfaces/        → DAL contracts (READ ONLY)
    Implementations/   → ← YOUR WORK (DAL stubs)
Etmen_BLL/
  DTOs/                → Data transfer objects (READ ONLY)
  Helpers/             → ServiceResult, BmiHelper, RiskCalculatorHelper (READ ONLY)
  Mapping/             → AutoMapper profile (extend if needed)
  Repositories/
    IServices/         → BLL contracts (READ ONLY)
    Services/          → ← YOUR WORK (BLL stubs)
Etmen_PL/
  Controllers/         → ← YOUR WORK (controller stubs)
  Views/               → ← FRONT-END (skip for now, stubs provided)
  Program.cs           → DI registration (all members update this)
```
