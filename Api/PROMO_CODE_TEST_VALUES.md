# Promo Code Test Values Guide

## Test Scenario 1: Basic Percentage Discount (Recommended for First Test)

### Form Values:
- **Promo Code:** `TEST2024` (or click "Generate" for random code)
- **Discount Type:** `Percentage (%)`
- **Discount Value:** `15`
- **Start Date:** Today's date (or tomorrow)
- **End Date:** 30 days from start date
- **Usage Limit (Total):** `100` (leave empty for unlimited)
- **Usage Limit Per User:** `1` (each user can use once)
- **Minimum Order Amount:** `50` (customer must order at least $50)
- **Maximum Discount Amount:** `25` (max discount is $25, even if 15% of order is more)
- **Description:** `Test promo code for 15% off orders over $50`

### Customer Selection:
- ✅ Select 1-2 registered users (e.g., "Alice Customer" or "System Administrator")
- ⚠️ Leave non-registered customers unchecked (they can't use it anyway)

### Email Addresses:
- Add test emails: `test@example.com`, `customer@test.com`
- These will receive the promo code but must register to use it

### Email Notification:
- ✅ Check "Send email notification to selected customers"

---

## Test Scenario 2: Fixed Amount Discount

### Form Values:
- **Promo Code:** `SAVE20` (or generate)
- **Discount Type:** `Fixed Amount ($)`
- **Discount Value:** `20`
- **Start Date:** Today
- **End Date:** 14 days from start
- **Usage Limit (Total):** `50`
- **Usage Limit Per User:** `2` (each user can use twice)
- **Minimum Order Amount:** `100`
- **Maximum Discount Amount:** Leave empty (not needed for fixed amount)
- **Description:** `$20 off orders over $100`

### Customer Selection:
- ✅ Select all registered users OR leave empty (allows all registered users)

### Email Addresses:
- Add: `newcustomer@example.com`

---

## Test Scenario 3: Unlimited Usage (No Restrictions)

### Form Values:
- **Promo Code:** `WELCOME10`
- **Discount Type:** `Percentage (%)`
- **Discount Value:** `10`
- **Start Date:** Today
- **End Date:** Leave empty (no expiration)
- **Usage Limit (Total):** Leave empty (unlimited)
- **Usage Limit Per User:** Leave empty (unlimited per user)
- **Minimum Order Amount:** Leave empty (no minimum)
- **Maximum Discount Amount:** Leave empty (no maximum)
- **Description:** `Welcome discount - 10% off everything`

### Customer Selection:
- Leave empty (allows ALL registered customers)

### Email Addresses:
- Add multiple emails: `customer1@test.com`, `customer2@test.com`, `customer3@test.com`

---

## Test Scenario 4: High-Value Promo Code

### Form Values:
- **Promo Code:** `BIGSAVE50`
- **Discount Type:** `Percentage (%)`
- **Discount Value:** `50`
- **Start Date:** Today
- **End Date:** 7 days from start (limited time offer)
- **Usage Limit (Total):** `10` (very limited)
- **Usage Limit Per User:** `1`
- **Minimum Order Amount:** `200`
- **Maximum Discount Amount:** `100` (caps discount at $100)
- **Description:** `Limited time: 50% off orders over $200 (max $100 discount)`

### Customer Selection:
- ✅ Select specific registered users (VIP customers)

---

## Quick Test Checklist:

1. ✅ **Create a promo code** with the values above
2. ✅ **Check email logs** in backend console (emails are logged, not actually sent)
3. ✅ **Verify registered users** can see the code in their account
4. ✅ **Test validation** - non-registered users should get "must register" message
5. ✅ **Test usage limits** - try using the code multiple times
6. ✅ **Test expiration** - create a code with past end date

---

## Example Values for Quick Copy-Paste:

**Basic Test:**
```
Code: TEST2024
Type: Percentage
Value: 15
Start: 2025-12-11
End: 2025-12-31
Usage Limit: 100
Per User: 1
Min Order: 50
Max Discount: 25
```

**Email Test:**
```
Emails to add:
- test@example.com
- customer@test.com
- newuser@demo.com
```

---

## What to Test:

1. **Creation:** Create promo code with test values
2. **Email Notifications:** Check backend logs for email content
3. **Customer Selection:** Verify only registered users can be selected
4. **Email Addresses:** Add emails and verify they're included
5. **Validation:** Try to use code as non-registered user (should fail)
6. **Usage:** Use code as registered user (should work)
7. **Limits:** Test usage limits (try using more than allowed)
8. **Expiration:** Create expired code and try to use it

---

## Expected Results:

✅ **Success:**
- Promo code created successfully
- Email notifications logged in backend
- Registered users can use the code
- Usage counts update correctly

❌ **Expected Failures:**
- Non-registered users cannot use codes
- Expired codes are rejected
- Usage limits are enforced
- Minimum order amounts are checked




