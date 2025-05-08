<h1 style="display: flex; align-items: center; margin: 0;">
  <img src="https://github.com/user-attachments/assets/30288d51-2f60-4e29-92b2-8a5881b4a788" alt="CROWD Logo" height="28" style="margin-right: 10px; vertical-align: 10px;">
  - Event Access Management
</h1>

The **CROWD** platform offers a comprehensive set of tools for event organizers, allowing efficient management of all access-related aspects, from event creation and participant management to ticket validation at entry and real-time monitoring.

## ✨ Key Features

*   🚀 **Simplified Event Management**: Easily create, configure (capacity, period, ticket types), modify, and archive events, and define granular permissions for total control.
*   🎟️ **Flexible Participant Management**: Add participants individually or quickly import large lists using standardized CSV files. Modify or cancel tickets directly from the control panel.
*   📊 **Real-Time Monitoring and Control**: Instantly view scan statuses, key statistics (tickets sold vs. scanned), and error reports directly in the centralized panel. Search for participants by name, email, or ticket code.
*   📲 **Fast and Secure Access Validation**: Allow entry teams (via the frontend/mobile app) to efficiently scan tickets (QR/barcode). The system instantly validates the ticket's uniqueness and correctness.
*   ⚙️ **Advanced Administrative Control**: Administrators can remotely start/stop scanning sessions, manually scan tickets directly from the panel, or cancel an erroneous scan, providing maximum flexibility in unforeseen situations.
*   🔔 **Proactive Notifications and Reporting**: Receive alerts and detailed reports from scanning terminals in case of errors or issues, allowing for rapid intervention.
*   📈 **Scalable Foundation**: The architecture based on .NET 8 and Entity Framework Core ensures performance and the ability to manage events of any size.
*   ⚡ **Live Updates**: Using SignalR, the admin panel receives real-time updates on scans and statistics without requiring a page reload.

## 🏗️ System Architecture

### Use Case Diagram

The use case diagram illustrates the main interactions between actors (Users, Administrators, Scanners) and the CROWD system's functionalities.
<p align="center">
  <img src="https://github.com/user-attachments/assets/99fd68d5-b5f3-419c-a3ad-75d377176464" alt="class_diag" width="700"/>
</p>

The class diagram details the main data structure and relationships between the system's key entities, such as Users, Events, Tickets, and Permissions. It provides an overview of how information is organized and interconnected within the database and business logic.
<p align="center">
  <img src="https://github.com/user-attachments/assets/404864d9-18c8-4f58-9435-b50d0a35b111" alt="class_diag" width="700"/>
</p>

### 🔌 CROWD v1 API Documentation

The **CROWD v1** backend API exposes the system's functionalities for client applications (web panel, mobile scanning app).

#### 👤 Admin

- `POST /api/admin/login`  
  Administrator login.

- `POST /api/admin`  
  Create administrator account.

#### 🧑 AppUser (Account)

- `POST /api/account/register`
  New user registration.

- `POST /api/account/login`
  User login (admin/scanner).

- `GET /api/account/checkUsername/{username}`  
  Check username availability.

- `GET /api/account/checkEmail/{email}`  
  Check email availability.


#### 🎉 Event

- `GET /api/event`  
  List events accessible to the user.

- `POST /api/event`
  Create new event.

- `GET /api/event/{eventCode}`  
  Event details (summary).

- `GET /api/event/{eventCode}/short`  
  Summarized event details.

- `GET /api/event/{eventCode}/full`
  Full event details (for panel).

- `DELETE /api/event/{eventId}`
  Delete event.

- `PUT /api/event/{eventId}`
  Update event.

- `PUT /api/event/{eventId}/scanningState/{state}`
  Change scanning state (start/stop).


#### 📄 Report

- `GET /api/report/{eventId}`
  List error reports for event.

- `POST /api/report/{eventId}`
  Submit new error report.


#### 🎟️ Ticket

- `GET /api/tickets/{eventId}`
  List tickets for event.

- `POST /api/tickets/{eventId}`
  Manually add ticket.

- `GET /api/tickets/{eventId}/csv`  
  Download ticket list in CSV format.

- `POST /api/tickets/{eventId}/csv`
  Upload ticket list from CSV.

- `GET /api/tickets/{eventId}/{code}`  
  Specific ticket details.

- `PUT /api/tickets/{eventId}/{code}`
  Update ticket details.

- `DELETE /api/tickets/{eventId}/{code}`
  Delete ticket.

- `PUT /api/tickets/{eventId}/{code}/scan`
  Mark ticket as scanned.

- `PUT /api/tickets/{eventId}/{code}/unscan`
  Cancel ticket scan.

## 🖥️ The CROWD Application Frontend

The **CROWD** application interface is built using a modern web stack, based on **React** and **TypeScript**, offering an intuitive and efficient user experience on both desktop and mobile devices.

### 🗂️ Application Structure

The frontend is organized into functional modules, each responsible for a set of pages or components. Navigation is handled via a `react-router-dom` router.

#### Web Page Map

- `/` – **Homepage**  
  Main page, where users can log in and see a description of the application.

- `/register` – **Register Page**  
  Page for user registration.

- `/events` – **Manage Events Page**  
  Page for managing events: listing, editing, deleting.

- `/events/create` – **Create Event Page**  
  Page for adding a new event.

- `/event/:eventCode` – **Event Page**  
  Page for managing an event.

- `/event/:eventCode/edit` – **Update Event Page**  
  Allows modification of event information.

- `/scan` – **Scan Page**  
  Page dedicated to ticket scanning, optimized for mobile use.


### 📸 Screenshots

<p align="center">
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/6b563e44-1b59-4bdc-b257-d3c526a9a3c7" width="49%"/>
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/e85ac4c7-4f9d-4297-ba7e-217992ded25c" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/a1a6b77d-7fc0-4404-8860-a87cd6dfccf0" width="49%"/>
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/21b6d167-3846-4a8a-acf6-1f99a2b92338" width="49%"/>
</p>
