# Email Setup Instructions

## Gmail SMTP Configuration

The email service is configured to use Gmail SMTP. To enable email sending, you need to:

### 1. Enable 2-Factor Authentication (Recommended)
1. Go to your Google Account settings: https://myaccount.google.com/
2. Navigate to Security
3. Enable 2-Step Verification

### 2. Generate an App Password
1. Go to: https://myaccount.google.com/apppasswords
2. Select "Mail" and "Other (Custom name)"
3. Enter "Kanabco API" as the name
4. Click "Generate"
5. Copy the 16-character app password (it will look like: `abcd efgh ijkl mnop`)

### 3. Add Password to Configuration

**Option A: appsettings.json (for development)**
```json
"Email": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "me5280908@gmail.com",
  "SenderName": "Kanabco",
  "SenderPassword": "your-16-character-app-password-here"
}
```

**Option B: Environment Variables (recommended for production)**
Set these environment variables:
- `Email__SenderPassword` = your app password

**Option C: User Secrets (for development)**
```bash
dotnet user-secrets set "Email:SenderPassword" "your-app-password-here"
```

### 4. Restart the Backend
After adding the password, restart the backend server.

## Testing

Once configured, when you create a promo code from the dashboard:
- All selected registered users will automatically receive emails
- The emails will be sent from `me5280908@gmail.com`
- The emails include the promo code, discount details, and expiration date

## Troubleshooting

If emails are not sending:
1. Check backend logs for error messages
2. Verify the app password is correct (no spaces, all 16 characters)
3. Ensure 2FA is enabled on the Gmail account
4. Check that "Less secure app access" is not required (use App Password instead)




