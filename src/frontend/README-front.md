# Events Management System - Frontend

Event management system developed in Angular with advanced authentication features, event management, data export, user profile page and implemented security measures.

## 🚀 Main Features

### 🔐 **Authentication and Security**
- User login and registration system
- **AES-256 encryption** for JWT tokens in localStorage
- Protection against XSS attacks through encryption of sensitive data
- Dedicated and reusable encryption service

### 👤 **User Profile Page**
- **Personalized view** with logged user information
- **Three main sections**:
  - **My Events**: Events created by the current user
  - **Public Events**: Public events from other users
  - **Registered Events**: Events the user is registered for
- **Independent pagination** for each section
- **Differentiated visual states** for each event type
- **Tab navigation** with event counters
- **Informative empty states** with call-to-action

### 📊 **Event Management**
- Event list with pagination (9 events per page)
- Category filtering
- Complete event details
- User registration to events
- Status indicators (full, registered, past)

### 📈 **Data Export**
- **Current page export**: CSV and Excel of visible events
- **Complete export**: CSV and Excel of ALL system events
- Intelligent category filtering in exports
- Security limits (maximum 1000 events)
- Visual progress during exports

### 🎨 **User Interface**
- Responsive design for mobile and desktop
- Visual status indicators
- Loading states with animated spinners
- User-friendly error handling

## 🔧 Technical Stack

- **Framework**: Angular 17
- **Language**: TypeScript
- **Styles**: CSS3 with responsive design
- **Libraries**: 
  - `crypto-js` for encryption
  - `xlsx` for Excel export
  - `file-saver` for file download
  - `Font Awesome` for iconography

## 📁 Project Structure

```
src/app/
├── components/                # Components folder
│   ├── create-event/          # Component for creating events
│   ├── event-detail/          # Component for event details
│   ├── events-list/           # Component for listing events
│   ├── export-buttons/        # Component for export buttons
│   ├── login/                 # Component for user login
│   ├── pagination/            # Component for pagination controls
│   ├── register/              # Component for user registration
│   └── user-profile/          # Component for user profile page
├── services/                  # Services folder
│   ├── auth.service.ts        # Service for authentication
│   ├── event.service.ts       # Service for event management
│   ├── export.service.ts      # Service for exporting data
│   └── encryption.service.ts  # Service for encryption
└── config/                    # Configuration folder
    └── encryption.config.ts   # Encryption configuration file
```

## 🔄 Development History


## 📄 Author

**Lorelay Pricop Florescu**  
Graduate in Interactive Technologies and Project Manager with experience in .NET, Python, Angular, Azure DevOps, AI, and Agile methodologies.

🔗 [LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
📧 Contact: lorelaypricop@gmail.com

> Some ideas were reviewed with the support of artificial intelligence (AI) tools

## 🔍 Problems Found and Solved

### 🚨 Critical
- [CRITICAL] **Category filter showed empty results**: When selecting a category in the event filter, the list showed empty results even though events existed in that category. The problem was due to poor handling of null values in the filter. Fixed by using `[ngValue]=null` instead of `[value]=null` in the `<option>` of the selector, ensuring the filter works correctly and shows the corresponding events.
- [CRITICAL] **Poor mobile layout and usability**: The interface was not fully responsive, making navigation and use difficult on mobile devices. The navbar, containers and event cards did not adapt properly to small screens, causing display and user experience issues. Fixed by rewriting CSS styles, adding media queries and improving component structure to ensure optimal experience on any device.
- [CRITICAL] **Lack of responsive pagination in event list**: The event list showed all results on a single page, making navigation and user experience difficult, especially on mobile devices. Implemented a fully responsive pagination system, with visual controls adapted to any screen size and support for pagination in both general view and category filters.
- [CRITICAL] **Unencrypted sensitive data in localStorage**: JWT tokens and user information were stored in localStorage in plain text, exposing sensitive data to potential XSS attacks or unauthorized access. Fixed by implementing AES-256 encryption with CryptoJS for all sensitive data stored in localStorage, using a dedicated encryption service.

### ⚠️ Important
- [IMPORTANT] **Lack of data export**: Users could not export event information to standard formats such as CSV or Excel, making external analysis and data portability difficult. Implemented export functionality in both CSV and Excel formats, allowing export of current page or all system events.
- [IMPORTANT] **Missing user profile page**: There was no dedicated page for users to view and manage their own events, public events and events they are registered for. Implemented a user profile page with tabs, counters and independent pagination for each section.
- [IMPORTANT] **Global event search**: Users could not search for events by title, description or creator from anywhere in the application. Implemented a global search component in the navigation bar, allowing instant and visual event filtering.
- [IMPORTANT] **Missing address field in events**: Users could not specify or view the address or location of an event, limiting available information and platform utility. Added the "address" field in event creation, viewing and export.
- [IMPORTANT] **Missing icons in event categories**: Event categories did not have a visual identifier, making quick distinction and user experience difficult. Added the "icon" field to categories and updated display in all relevant components.
- [IMPORTANT] **Inability to cancel event registration**: Users could not cancel their registration in an event once registered, causing frustration and lack of control over their participation. Implemented registration cancellation functionality from event list, event detail and user profile.
- [IMPORTANT] **Inability to view other users' profiles and events**: Users could only access their own profile and could not consult events created by other users. Implemented dual profile functionality, allowing viewing of both own profile and any other user's profile, along with their events.
- [IMPORTANT] **Inability to delete created events**: Users could not delete events they had created themselves, making management and cleanup of old or erroneous events difficult. Added a delete button in event list, detail and user profile, allowing safe deletion of own events with confirmation.

### 💡 Implemented Improvements

- **Advanced Event Management and Profile**:
  - User profile page with tabs for "My Events", "Public Events" and "Registered Events", each with pagination, counters and differentiated visual states.
  - Dual profile: ability to view own profile or any user's profile, with direct navigation from event lists and details.
  - Component consolidation: all profile and event display logic is unified in a single reusable component.
  - Informative empty states and contextual messages according to profile type or section.

- **Data Export**:
  - Event export in CSV or Excel format from any section (current page, all events, profile).
  - Grouped and visually clear export section, with compact buttons, icons and responsive design.
  - Security limits for bulk exports and visual progress bar.

- **User Interaction and Control**:
  - Delete event button available in all relevant views (list, detail, profile), with confirmation and automatic view updates.
  - Event registration cancellation functionality from any view, with immediate visual feedback.

- **Search and Filtering**:
  - Reusable global search component, integrated in navigation bar, with debounce and quick clearing.
  - Support for search from both backend and frontend (client and server filtering).
  - Search integration with routes and pagination, allowing persistent searches and fluid navigation.

- **Visualization and Usability**:
  - Icons in event categories, integrated in selectors, labels and badges for quick identification.
  - Address field in events, visible in creation, details and export.
  - Fully responsive navbar and optimized mobile search.
  - Header reorganization and export buttons in event detail view.
  - Visual and style adjustments in buttons, animations and layout to improve experience on all devices.

### ⏱️ Time Invested

- **Analysis and Design**:  
  Bug analysis, requirements, user experience and architecture: **1 hours**

- **Implementation and Development**:  
  Feature implementation (export, profile, search, cancellation, deletion, profile duality, etc.): **3 hours**
  Refactoring, component consolidation and layout/style adjustments: **2 hours**

- **Testing and Validation**:  
  Testing on different devices, feature validation, visual feedback and final adjustments: **1 hours**

- **Total approximate**: **~7 hours**

## 📄 Author

**Lorelay Pricop Florescu**  
Graduate in Interactive Technologies and Project Manager with experience in .NET, Python, Angular, Azure DevOps, AI, and Agile methodologies.

🔗 [LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
📧 Contact: lorelaypricop@gmail.com

> Some ideas were reviewed with the support of artificial intelligence (AI) tools