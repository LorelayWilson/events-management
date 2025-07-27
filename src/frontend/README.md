# Events Management System - Frontend

## 📋 Overview

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


### **Commit #1: Category Filters Fix**
**Hash**: fe6e1ce  
**Type**: Bug Fix  
**Identified problem**: Category filters showed empty results when selecting a category. This happened when changing from a specific category to "All Categories".  
**Root cause**: Incorrect binding usage in Angular (`[value]` instead of `[ngValue]`)  
**Implemented solution**: 
- Change from `[value]="null"` to `[ngValue]="null"` for "All Categories" option
- Change from `[value]="category.id"` to `[ngValue]="category.id"` for individual categories
- Correction of bidirectional binding in template

**Modified files**:
- `src/app/components/events-list/events-list.component.html`

**Result**: Completely functional category filters

---

### **Commit #2: Responsive Layout Improvement**
**Hash**: 1161f51  
**Type**: UI/UX Improvement  
**Identified problem**: Poor layout on mobile devices and usability issues  
**Implemented solution**:
- Improvement of responsive design for mobile devices
- Optimization of user experience on small screens
- Correction of layout and navigation problems

**Modified files**:
- Component and style files to improve responsiveness

**Result**: Better user experience on mobile devices

---

### **Commit #3: Pagination Implementation**
**Hash**: 5816331  
**Type**: New Feature  
**Implemented functionality**: Pagination system for events list  
**Developed features**:
- Frontend pagination to display events by pages
- Integration with backend that already had pagination implemented
- Navigation between event pages
- Performance improvement when loading events

**Modified files**:
- `src/app/components/events-list/events-list.component.ts` - Pagination logic and navigation
- `src/app/components/events-list/events-list.component.html` - Pagination controls UI
- `src/app/components/events-list/events-list.component.css` - Styles for pagination controls
- `src/app/services/event.service.ts` - Integration with backend pagination APIs

**Result**: Paginated events list with better performance

---

### **Commit #4: LocalStorage Encryption**
**Hash**: 59f86bc  
**Type**: Security Enhancement  
**Identified problem**: Critical XSS security vulnerability - JWT tokens stored in plain text in localStorage  
**Implemented solution**:
- AES-256 encryption implementation using CryptoJS
- Creation of dedicated encryption service
- Centralized encryption key configuration
- Robust error handling with automatic cleanup of corrupted data
- Identification prefixes for encrypted values

**Created files**:
- `src/app/config/encryption.config.ts` - Centralized encryption configuration
- `src/app/services/encryption.service.ts` - Encryption/decryption service

**Modified files**:
- `src/app/services/auth.service.ts` - Integration with encryption service

**Security improvements**:
- Automatic encryption of tokens and user data
- Integrity validation of decrypted data
- Automatic cleanup of corrupted data
- Secure configuration for production

---

### **Commit #5: Export Functionality**
**Hash**: d4b451b  
**Type**: New Feature  
**Implemented functionality**: Complete CSV and Excel export system  
**Developed features**:
- Individual event export from detail page
- Event list export (current page)
- Detailed format for Excel with statistics and metadata
- Automatic file naming with dates
- Intelligent category filtering in exports

**Created files**:
- `src/app/services/export.service.ts` - Main export service

**Modified files**:
- `src/app/components/events-list/events-list.component.ts` - List export integration
- `src/app/components/events-list/events-list.component.html` - Export buttons
- `src/app/components/events-list/events-list.component.css` - Export button styles
- `src/app/components/event-detail/event-detail.component.ts` - Individual export integration
- `src/app/components/event-detail/event-detail.component.html` - Individual export buttons
- `src/app/components/event-detail/event-detail.component.css` - Export styles

**Technologies used**:
- `xlsx` for Excel file generation
- `file-saver` for file download
- Native CSV format for text files

**Result**: Complete and functional data export system

---

### **Commit #6: User Profile Page & Export Functionality & Project Restructuring**
**Hash**: [Pending]  
**Type**: New Feature & Code Refactoring  
**Implemented functionality**: Complete user profile page with personal event management, reusable export component, and improved project structure  
**Developed features**:

#### **Main Component**:
- **UserProfileComponent**: Centralized page for user event management
- **Personalized header**: User information with avatar and quick actions
- **Tab navigation**: Intuitive interface to switch between sections
- **Dynamic counters**: Number of events in each tab

#### **Three Main Sections**:

1. **My Events**:
   - Events created by the current user
   - Visual indicator "Created by you"
   - Capacity and registration information
   - Privacy status (public/private)
   - Empty state with call to create first event
   - **Export functionality**: CSV and Excel export for my events

2. **Public Events**:
   - Public events from other users
   - Event creator information
   - Registration button for unregistered events
   - "Registered" indicator for already enrolled events
   - Informative empty state
   - **Export functionality**: CSV and Excel export for public events

3. **Registered Events**:
   - Events the user is registered for
   - Visual indicator "Registered"
   - Creator and capacity information
   - Empty state with link to public events
   - **Export functionality**: CSV and Excel export for registered events

#### **Export Buttons Component**:
- **ExportButtonsComponent**: Reusable component for CSV and Excel export buttons
- **Configurable inputs**: Label, button size, disabled states, tooltips
- **Event emitters**: `exportCsv` and `exportExcel` events
- **Responsive design**: Adapts to mobile and desktop layouts
- **Consistent styling**: Matches application design system
- **Used across all pages**: User profile, events list, and event detail

#### **Technical Features**:
- **Frontend filtering**: Uses existing APIs with client-side filtering
- **Independent pagination**: Each section maintains its own page state
- **Reusable pagination component**: PaginationComponent with modern design
- **Extended service methods**:
  - `getMyEvents()`: Filters events created by current user
  - `getPublicEvents()`: Filters public events from other users
  - `getRegisteredEvents()`: Filters events where user is registered
- **Loading states**: Animated spinners during requests
- **Error handling**: Detailed logs and error states

#### **API Integration**:
- **Uses existing endpoints**: Leverages `/events` API with frontend filtering
- **Efficient data loading**: Loads 50 events per page for better filtering
- **User-based filtering**: Filters events based on `createdById` and `isRegistered`
- **Privacy filtering**: Separates public and private events appropriately

#### **Design and UX**:
- **Responsive design**: Adapted for mobile and desktop
- **Modern gradients**: Attractive gradient header
- **Differentiated cards**: Color borders according to event type
  - Green: My events
  - Blue: Public events
  - Orange: Registered events
- **Font Awesome iconography**: Descriptive icons throughout the interface
- **Informative empty states**: Useful messages with suggested actions

#### **Integrated Navigation**:
- **Navbar link**: "My Profile" with user icon
- **Protected route**: `/profile` only accessible for logged users
- **Automatic redirect**: To login if no active session
- **Quick actions**: Buttons to create event and logout

#### **Project Structure Improvements**:
- **Organized components**: Each component in its own subfolder
- **Clean architecture**: Better separation of concerns
- **Maintainable codebase**: Easier to locate and modify files
- **Scalable structure**: Ready for project growth

**Created files**:
- `src/app/components/user-profile/user-profile.component.ts` - Main profile logic
- `src/app/components/user-profile/user-profile.component.html` - Template with tabs and events
- `src/app/components/user-profile/user-profile.component.css` - Modern and responsive styles
- `src/app/components/pagination/pagination.component.ts` - Reusable pagination component
- `src/app/components/pagination/pagination.component.html` - Pagination template
- `src/app/components/pagination/pagination.component.css` - Pagination styles
- `src/app/components/export-buttons/export-buttons.component.ts` - Reusable export component logic
- `src/app/components/export-buttons/export-buttons.component.html` - Export buttons template
- `src/app/components/export-buttons/export-buttons.component.css` - Export buttons styles

**Restructured folders**:
- `src/app/components/create-event/` - CreateEventComponent files
- `src/app/components/event-detail/` - EventDetailComponent files
- `src/app/components/events-list/` - EventsListComponent files
- `src/app/components/export-buttons/` - ExportButtonsComponent files
- `src/app/components/login/` - LoginComponent files
- `src/app/components/pagination/` - PaginationComponent files
- `src/app/components/register/` - RegisterComponent files
- `src/app/components/user-profile/` - UserProfileComponent files

**Modified files**:
- `src/app/services/event.service.ts` - Frontend filtering methods using existing APIs
- `src/app/app.module.ts` - New component registration and updated import paths
- `src/app/app-routing.module.ts` - New `/profile` route and updated import paths
- `src/app/app.component.html` - Profile navigation link
- `src/index.html` - Font Awesome integration for icons
- `src/app/components/user-profile/user-profile.component.html` - Integrated export buttons component
- `src/app/components/events-list/events-list.component.html` - Integrated export buttons component
- `src/app/components/event-detail/event-detail.component.html` - Integrated export buttons component
- All component files updated with correct import paths for services

**Technologies used**:
- Angular 17 with TypeScript
- CSS Grid and Flexbox for layouts
- Font Awesome 6.4.0 for iconography
- CSS gradients for visual effects
- Responsive design with media queries
- RxJS operators for frontend filtering
- Component-based architecture
- Modern CSS with custom properties

**Result**: Complete and functional profile page with export functionality that allows users to manage all their events from a centralized and modern interface, using existing APIs with efficient frontend filtering. Clean, maintainable, and scalable codebase with reusable export functionality and improved project organization. All export features now use a consistent, reusable component across the entire application.

---

## 📊 Performance Metrics

- **Current page export**: Instantaneous
- **Complete export**: Optimized (50 event chunks)
- **Security limit**: 1000 events maximum
- **Encryption**: Automatic AES-256
- **Delay between calls**: 100ms
- **Pagination**: 50 events per page in user profile (frontend filtered)


## 📚 Documentation

Detailed documentation of implemented features can be found in the development history of this README. Specific documentation files (`SECURITY.md`, `EXPORT_FEATURES.md`, `EXPORT_COMPLETE_FEATURES.md`) were removed to keep documentation centralized in this file.

## **Upcoming Improvements**
- [ ] Real-time export with WebSockets
- [ ] Large file compression
- [ ] Statistics dashboard
- [ ] Progressive Web App (PWA)
- [ ] Complete automated testing
- [ ] Real-time notifications
- [ ] Improved private event management

## 📄 Author

**Lorelay Pricop Florescu**  
Graduate in Interactive Technologies and Project Manager with experience in .NET, Python, Angular, Azure DevOps, AI, and Agile methodologies.

🔗 [LinkedIn](https://www.linkedin.com/in/lorelaypricop)  
📧 Contact: lorelaypricop@gmail.com

> Some ideas were reviewed with the support of artificial intelligence (AI) tools