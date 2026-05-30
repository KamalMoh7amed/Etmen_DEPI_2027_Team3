# 📋 Etmen DEPI Project — Task Distribution Board

> **Project:** Etmen Health Platform  
> **Team:** 4 Members (بعد إعادة التوزيع)  
> **Focus:** Backend Implementation Only (BLL Services + DAL Repositories + PL Controllers)  
> **Date:** 2026  
> **آخر تحديث:** شغل كريم اتضاف لعبدالرحمن

---

## 👥 Team Members & Skill Levels

| # | Name | Skill Level | Role |
|---|------|-------------|------|
| 1 | **كمال محمد صابر** | ⭐⭐⭐⭐⭐ Expert | 🎯 Team Lead + Complex Tasks |
| 2 | **بهنساوي** | ⭐⭐⭐⭐⭐ Advanced | 🔥 High-Complexity Tasks |
| 3 | **عبد الحميد** | ⭐⭐⭐⭐⭐ Advanced | 🔥 High-Complexity Tasks |
| 4 | **عبدالرحمن** | ⭐⭐⭐ Intermediate | Foundation + Standard Tasks |
| ~~5~~ | ~~كريم~~ | ~~⭐⭐⭐ Intermediate~~ | ~~تم نقل مهامه لعبدالرحمن~~ |

---

## 🗂️ Task Distribution (By Complexity)

### 👤 كمال محمد صابر — Group 5: AI Chat, Notifications, Family & Admin ⚡🧠
> **Highest Complexity** — External APIs, Background Jobs, Role Management

#### 📁 Services (BLL)
| File | Methods | Complexity |
|------|---------|------------|
| `AlertService.cs` | All methods (8) | 🟡 Medium |
| `NotificationService.cs` | All methods (7) + Background Job | 🟡 Medium |
| `FamilyService.cs` | All methods (7) + Token Generation | 🔴 High |
| `AIChatService.cs` | All methods (6) + External LLM API | 🔴🔴 Very High |
| `AdminService.cs` | All methods + Role Authorization | 🟡 Medium |

#### 🎮 Controllers (PL)
| File | Actions |
|------|---------|
| `ChatController.cs` | `Index`, `Send`, `ClearHistory` |
| `PatientController.cs` | `Alerts` |

#### 🗄️ DAL Repositories
| File | Methods Count |
|------|--------------|
| `AlertRepository.cs` | 8 |
| `NotificationRepository.cs` | 7 |
| `FamilyLinkRepository.cs` | 7 |
| `ChatMessageRepository.cs` | 6 |

> 🔑 **Key Challenges for كمال:**
> - `AIChatService`: Integrate external LLM API with conversation context (last 10 messages)
> - `FamilyService`: Generate secure `Guid` invite tokens + send via `INotificationService`
> - `NotificationService`: Wire `ClearExpiredNotificationsAsync` with `IHostedService`
> - Admin role enforcement with `[Authorize(Roles = "Admin")]`
> - Manage `appsettings.json` for API keys and background job configs

---

### 👤 بهنساوي — Group 4: Crisis, Emergency & Geo Services 🚨🌍
> **High Complexity** — Geospatial logic, crisis state management, emergency routing

#### 📁 Services (BLL)
| File | Methods |
|------|---------|
| `CrisisService.cs` | All methods |
| `CrisisRiskEngineService.cs` | All methods |
| `EmergencyService.cs` | All methods |
| `NearbyService.cs` | All methods |

#### 🎮 Controllers (PL)
| File | Actions |
|------|---------|
| `PatientController.cs` | `Nearby` |

#### 🗄️ DAL Repositories
| File | Methods Count |
|------|--------------|
| `CrisisConfigurationRepository.cs` | 9 |
| `OutbreakZoneRepository.cs` | 6 |
| `EmergencyRequestRepository.cs` | 10 |
| `HealthcareProviderRepository.cs` | 6 |

> 🔑 **Key Implementation Notes:**
> - Use `GeoHelper.HaversineDistance()` for all location-based queries
> - Enforce single active crisis in `ActivateCrisisAsync` (check + deactivate others)
> - `EmergencyService.CreateEmergencyRequestAsync`: Auto-find nearest available provider using geo-sorting
> - Crisis risk scoring: Load `SymptomWeights` from active crisis entity

---

### 👤 عبد الحميد — Group 3: Medical Records, Labs & Risk Engine 🏥🧬
> **High Complexity** — Business logic, file handling, risk calculations

#### 📁 Services (BLL)
| File | Methods |
|------|---------|
| `MedicalRecordService.cs` | All methods |
| `LabService.cs` | All methods + File Upload |
| `RiskService.cs` | All methods |
| `PatientService.cs` | 6 additional methods |

#### 🎮 Controllers (PL)
| File | Actions |
|------|---------|
| `PatientController.cs` | `MedicalRecords`, `AddMedicalRecord`, `RiskAssessment` (GET+POST), `RiskResult`, `LabResults` |

#### 🗄️ DAL Repositories
| File | Methods Count |
|------|--------------|
| `MedicalRecordRepository.cs` | 5 |
| `LabResultRepository.cs` | 7 |
| `RiskAssessmentRepository.cs` | 7 |

> 🔑 **Key Implementation Notes:**
> - Use `RiskCalculatorHelper.CalculateScore()` in `RiskService.CalculateRiskAsync`
> - Use `BmiHelper.Calculate()` when creating medical records
> - Lab file upload: Save to `wwwroot/uploads/labs/`, store relative path in entity
> - OCR for lab files is optional/bonus feature

---

### 👤 عبدالرحمن — Group 1 + Group 2: Auth, Patient Profile, Doctors & Appointments 🔐👤🩺📅
> **Foundation + Standard Tasks** — ⚠️ 

---

#### 🔐 Group 1: Auth & Patient Profile

##### 📁 Services (BLL)
| File | Methods | Complexity |
|------|---------|------------|
| `AuthService.cs` | `RegisterAsync`, `LoginAsync`, `VerifyEmailAsync`, `ForgotPasswordAsync`, `ResetPasswordAsync`, `DeactivateAccountAsync`, `IsEmailTakenAsync` | 🟢 Foundation |
| `PatientService.cs` | `GetProfileAsync`, `UpdateProfileAsync`, `GetDashboardAsync` | 🟢 Foundation |

##### 🎮 Controllers (PL)
| File | Actions |
|------|---------|
| `AccountController.cs` | `Register`, `Login`, `Logout`, `VerifyEmail`, `ForgotPassword`, `ResetPassword` |
| `PatientController.cs` | `Dashboard`, `Profile` (GET + POST) |

##### 🗄️ DAL Repositories
| File | Methods Count |
|------|--------------|
| `PatientProfileRepository.cs` | 7 |

> 💡 **Implementation Tips (Group 1):**
> - Use injected `UserManager<ApplicationUser>` and `SignInManager` for auth operations
> - JWT token generation in `AuthService.LoginAsync`: read signing key from `appsettings.json`
> - `GetDashboardAsync`: Aggregate upcoming appointments + latest risk assessment + unread alerts count
> - Email confirmation tokens: use `UserManager.GenerateEmailConfirmationTokenAsync()`

---

#### 🩺 Group 2: Doctors & Appointments *(نُقل من كريم)*

##### 📁 Services (BLL)
| File | Methods | Complexity |
|------|---------|------------|
| `DoctorService.cs` | All methods | 🟡 Medium |
| `AppointmentService.cs` | All methods | 🟡 Medium |

##### 🎮 Controllers (PL)
| File | Actions |
|------|---------|
| `DoctorController.cs` | All actions |
| `PatientController.cs` | `Appointments`, `CancelAppointment` |

##### 🗄️ DAL Repositories
| File | Methods Count |
|------|--------------|
| `DoctorProfileRepository.cs` | 6 |
| `AppointmentRepository.cs` | 8 |
| `AvailableSlotRepository.cs` | 6 |

> ⚠️ **Critical Requirements (Group 2):**
> - `BookAppointmentAsync` MUST be transactional:
>   ```csharp
>   using var transaction = await _unitOfWork.BeginTransactionAsync();
>   try {
>       // Check slot → Create Appointment → Mark slot booked
>       await _unitOfWork.CommitTransactionAsync();
>   } catch {
>       await _unitOfWork.RollbackTransactionAsync();
>       throw;
>   }
>   ```
> - `BulkAddSlotsAsync`: Generate slots from date range + time intervals (e.g., every 30 mins)
> - Apply `[Authorize(Roles = "Doctor")]` on all `DoctorController` actions

---

## ✅ Shared Checklist (All Members)

### Before Starting
- [ ] Pull latest code from main branch
- [ ] Read all `// TODO` comments in your assigned stub files carefully

### During Implementation
- [ ] Replace `throw new NotImplementedException();` with real logic
- [ ] Register your service interface + implementation in `Program.cs` using `AddScoped`
- [ ] Verify AutoMapper mappings in `BLLMappingProfile.cs` include your DTOs
- [ ] Do NOT modify interfaces, DTOs, entities, or EF configurations — implement stubs only

### Before Pushing
- [ ] Run `dotnet build` — ensure zero compile errors
- [ ] Write at least one manual test OR unit test per implemented method
- [ ] Test edge cases: null inputs, unauthorized access, duplicate data
- [ ] Add XML comments for public methods (optional but recommended)

### PR Process
- [ ] Open one PR per task group (e.g., `"feat: implement Group 1+2 - Auth & Appointments"`)
- [ ] Tag @كمال محمد صابر for code review
- [ ] Address review comments before merge
- [ ] Update this board after PR merge ✅

---

## 🔄 Workflow

```
Pull latest code
      ↓
Read // TODO comments
      ↓
Implement stub methods
      ↓
dotnet build + manual test
      ↓
Write unit tests if possible
      ↓
Open PR + tag reviewer
      ↓
Code review by كمال
      ↓
  Approved? ──No──→ Fix + re-request review
      │
     Yes
      ↓
  Merge to main ✅
```
