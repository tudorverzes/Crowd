<h1 style="display: flex; align-items: center; margin: 0;">
  <img src="https://github.com/user-attachments/assets/30288d51-2f60-4e29-92b2-8a5881b4a788" alt="CROWD Logo" height="28" style="margin-right: 10px; vertical-align: 10px;">
  - Gestionare Acces Evenimente
</h1>



Platforma **CROWD** oferă un set complet de unelte pentru organizatorii de evenimente, permițând administrarea eficientă a tuturor aspectelor legate de acces, de la crearea evenimentului și gestionarea participanților, până la validarea biletelor la intrare și monitorizarea în timp real.

## ✨ Funcționalități Cheie

*   🚀 **Management Simplificat al Evenimentelor**: Creați, configurați (capacitate, perioadă, tipuri de bilete), modificați și arhivați evenimente cu ușurință și definiți permisiuni granulare pentru un control total.
*   🎟️ **Gestionare Flexibilă a Participanților**: Adăugați participanți individual sau importați liste mari rapid, folosind fișiere CSV standardizate. Modificați sau anulați bilete direct din panoul de control.
*   📊 **Monitorizare și Control în Timp Real**: Vizualizați instantaneu statusul scanărilor, statistici cheie (bilete vândute vs. scanate) și rapoarte de eroare direct în panoul centralizat. Căutați participanți după nume, email sau cod de bilet.
*   📲 **Validare Rapidă și Sigură la Acces**: Permiteți echipelor de la intrare (prin intermediul aplicației frontend/mobile) să scaneze biletele (QR/cod bare) eficient. Sistemul validează instantaneu unicitatea și corectitudinea biletului.
*   ⚙️ **Control Administrativ Avansat**: Administratorii pot porni/opri remote sesiunile de scanare, pot scana manual bilete direct din panou sau anula o scanare eronată, oferind flexibilitate maximă în situații neprevăzute.
*   🔔 **Notificări și Raportare Proactivă**: Primiți alerte și rapoarte detaliate de la terminalele de scanare în cazul unor erori sau probleme, permițând intervenția rapidă.
*   📈 **Fundație Scalabilă**: Arhitectura bazată pe .NET 8 și Entity Framework Core asigură performanță și capacitatea de a gestiona evenimente de orice mărime.
*   ⚡ **Actualizări Live**: Folosind SignalR, panoul de administrare primește actualizări în timp real despre scanări și statistici, fără a necesita reîncărcarea paginii.

## 🏗️ Arhitectura Sistemului

### Diagrama Cazuri de Utilizare

Diagrama cazurilor de utilizare ilustrează interacțiunile principale dintre actori (Utilizatori, Administratori, Scaneri) și funcționalitățile sistemului CROWD.

<p align="center">
  <img src="https://github.com/user-attachments/assets/404864d9-18c8-4f58-9435-b50d0a35b111" alt="class_diag" width="700"/>
</p>
Diagrama de clase detaliază structura principală a datelor și relațiile dintre entitățile cheie ale sistemului, cum ar fi Utilizatorii, Evenimentele, Biletele și Permisiunile. Aceasta oferă o imagine de ansamblu asupra modului în care informațiile sunt organizate și interconectate în cadrul bazei de date și al logicii de business.

### 🔌 Documentație API CROWD v1

API-ul backend **CROWD v1** expune funcționalitățile sistemului pentru aplicațiile client (panou web, aplicație mobilă de scanare).

#### 👤 Admin

- `POST /api/admin/login`  
  Autentificare administrator.

- `POST /api/admin`  
  Creare cont administrator.

#### 🧑 AppUser (Account)

- `POST /api/account/register`
  Înregistrare utilizator nou.

- `POST /api/account/login`
  Autentificare utilizator (admin/scanner).

- `GET /api/account/checkUsername/{username}`  
  Verificare disponibilitate username.

- `GET /api/account/checkEmail/{email}`  
  Verificare disponibilitate email.


#### 🎉 Event

- `GET /api/event`  
  Listare evenimente accesibile utilizatorului.

- `POST /api/event`
  Creare eveniment nou.

- `GET /api/event/{eventCode}`  
  Detalii (sumar) eveniment.

- `GET /api/event/{eventCode}/short`  
  Detalii sumarizate eveniment.

- `GET /api/event/{eventCode}/full`
  Detalii complete eveniment (pentru panou).

- `DELETE /api/event/{eventId}`
  Ștergere eveniment.

- `PUT /api/event/{eventId}`
  Actualizare eveniment.

- `PUT /api/event/{eventId}/scanningState/{state}`
  Modificare stare scanare (pornit/oprit).


#### 📄 Report

- `GET /api/report/{eventId}`
  Listare raportări eroare pentru eveniment.

- `POST /api/report/{eventId}`
  Trimitere raportare eroare nouă.


#### 🎟️ Ticket

- `GET /api/tickets/{eventId}`
  Listare bilete pentru eveniment.

- `POST /api/tickets/{eventId}`
  Adăugare manuală bilet.

- `GET /api/tickets/{eventId}/csv`  
  Descărcare listă bilete în format CSV.

- `POST /api/tickets/{eventId}/csv`
  Încărcare listă bilete din CSV.

- `GET /api/tickets/{eventId}/{code}`  
  Detalii bilet specific.

- `PUT /api/tickets/{eventId}/{code}`
  Actualizare detalii bilet.

- `DELETE /api/tickets/{eventId}/{code}`
  Ștergere bilet.

- `PUT /api/tickets/{eventId}/{code}/scan`
  Marcare bilet ca scanat.

- `PUT /api/tickets/{eventId}/{code}/unscan`
  Anulare scanare bilet.

## 🖥️ Frontend-ul aplicației CROWD

Interfața aplicației **CROWD** este realizată folosind un stack modern web, bazat pe **React** și **TypeScript**, oferind o experiență de utilizare intuitivă și eficientă atât pe desktop, cât și pe dispozitive mobile.

### 🗂️ Structura aplicației

Frontend-ul este organizat în module funcționale, fiecare responsabil de un set de pagini sau componente. Navigarea se face printr-un intermediul unui router `react-router-dom`.

#### Harta paginilor web

- `/` – **Homepage**  
  Pagina principală, unde utilizatorii se pot autentifica și pot vedea o descriere a aplicației.

- `/register` – **Register Page**  
  Pagină pentru înregistrarea utilizatorilor.

- `/events` – **Manage Events Page**  
  Pagina pentru gestionarea evenimentelor: listare, editare, ștergere.

- `/events/create` – **Create Event Page**  
  Pagină pentru adăugarea unui eveniment nou.

- `/event/:eventCode` – **Event Page**  
  Pagina de gestionare a unui eveniment.

- `/event/:eventCode/edit` – **Update Event Page**  
  Permite modificarea informațiilor unui eveniment.

- `/scan` – **Scan Page**  
  Pagina dedicată scanării biletelor, optimizată pentru uz mobile.


### 📸 Capturi de ecran

<p align="center">
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/6b563e44-1b59-4bdc-b257-d3c526a9a3c7" width="49%"/>
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/e85ac4c7-4f9d-4297-ba7e-217992ded25c" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/a1a6b77d-7fc0-4404-8860-a87cd6dfccf0" width="49%"/>
  <img src="https://github.com/tudorvezes/Crowd/assets/112432315/21b6d167-3846-4a8a-acf6-1f99a2b92338" width="49%"/>
</p>

