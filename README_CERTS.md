# 🛡️ Security Manual: Secrets and Certificates

This project utilizes **Docker Secrets** for sensitive credentials and **SSL Encryption** for all communications with the PostgreSQL database. These files are strictly excluded from the repository via `.gitignore`.

---

## 📂 Directory Structure

To ensure the system functions correctly, you must manually create two directories in the project root:

- **/secrets** — Contains database credentials (shared with `db` and `silkaraserver`).
- **/certs** — Contains SSL certificates (required by the `db` container).

---

## 🔑 1. Configuring Secrets

Create the `/secrets` folder and add the following two files:

1.  `db_user.txt` — The database superuser login (e.g., `admin`).
2.  `db_password.txt` — A strong, secure password.

> [!IMPORTANT]
> These files must contain **only the raw string**. Do not include extra spaces, quotes, or trailing newlines.

---

## 📜 2. SSL Certificate Generation

The database is configured to enforce SSL. To generate self-signed certificates for internal use, follow these steps:

### Step A: Generate Files

Navigate to the `/certs` directory and execute the following command:

```bash
openssl req -new -x509 -days 365 -nodes -out server.crt -keyout server.key -subj "//CN=db"
```
