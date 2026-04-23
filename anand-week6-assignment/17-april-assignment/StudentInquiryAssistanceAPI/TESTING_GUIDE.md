# Student Inquiry Assistance API Testing Guide

Run the API:

```powershell
dotnet run
```

Open Swagger:

```text
http://localhost:8080/swagger
```

Postman files:

- Collection: [Postman/StudentInquiryAssistanceAPI.postman_collection.json](C:/Users/AnandThakur/Desktop/gh-repo/anand-cg-assignments/anand-week6-assignment/17-april-assignment/StudentInquiryAssistanceAPI/Postman/StudentInquiryAssistanceAPI.postman_collection.json)
- Environment: [Postman/StudentInquiryAssistanceAPI.postman_environment.json](C:/Users/AnandThakur/Desktop/gh-repo/anand-cg-assignments/anand-week6-assignment/17-april-assignment/StudentInquiryAssistanceAPI/Postman/StudentInquiryAssistanceAPI.postman_environment.json)

Import both into Postman, select the local environment, then run the requests in folder order: `Auth`, `Course`, `Student`, `Admission`, `Enquiry`, `Payment`.

## Seeded Accounts

- Admin: `admin@niit.com` / `Admin@123`
- Office Staff: `staff@niit.com` / `Staff@123`
- Student: `student1@niit.com` / `Stud123`

## Seeded Courses

- `Java Full Stack`
- `.NET Full Stack`
- `Data Analytics`

## Recommended Test Flow

1. Login as admin with `POST /auth/login`.
2. Copy the token and authorize Swagger with `Bearer <token>`.
3. Check `GET /api/course`.
4. Create a course using `POST /api/course`.
5. Login as student and authorize with the student token.
6. Check `GET /api/student/me` and `GET /api/student/course`.
7. Create an admission using `POST /api/admission` with `courseId: 1`.
8. Create an enquiry using `POST /api/enquiry`.
9. Repeat the enquiry request 5 times total and confirm the 6th request returns `400 BadRequest`.
10. Make a payment using `POST /api/payment` with `admissionId: 1`.
11. Check `GET /api/payment/history/me`.
12. Login as office staff and authorize with the staff token.
13. Check `GET /api/enquiry`.
14. Reply to an enquiry using `PATCH /api/enquiry/{id}/status`.
15. Check `GET /api/payment`.
16. Check `GET /api/payment/balance/{admissionId}`.
17. Trigger a reminder using `POST /api/payment/reminder/{admissionId}`.

## Sample Request Bodies

Admin create course:

```json
{
  "courseName": "Cloud Computing",
  "description": "Cloud foundations, deployment models, and Azure services.",
  "instructorName": "Cloud Trainer",
  "duration": "3 Months",
  "feesAmount": 28000
}
```

Student create admission:

```json
{
  "courseId": 1
}
```

Student create enquiry:

```json
{
  "courseId": 1,
  "title": "Need fee details",
  "description": "Please share installment options and class schedule.",
  "enquiryType": "Fees"
}
```

Office staff reply to enquiry:

```json
{
  "status": "Replied",
  "responseMessage": "Installments are available. We will share the full fee plan shortly."
}
```

Student make payment:

```json
{
  "admissionId": 1,
  "amount": 10000,
  "paymentMode": "UPI"
}
```

## Quick Validation Checks

- Unauthenticated access to protected endpoints should return `401`.
- Student should not be able to access admin-only endpoints.
- Payment amount greater than balance should return `400`.
- Duplicate admission for the same course and student should return `400`.
- Deleting a course with related admissions, enquiries, or payments should return `400`.
- Handled errors should be recorded in `ErrorLogs`.
