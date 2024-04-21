import express from 'express';
import session from 'express-session';
import Keycloak from 'keycloak-connect';

const app = express();

const memoryStore = new session.MemoryStore();
app.use(session({
    secret: 'some long secret',
    resave: false,
    saveUninitialized: true,
    store: memoryStore
}));
const keycloak = new Keycloak({ store: memoryStore });

app.use(keycloak.middleware({
    logout: '/logout'
}));

// Public route
app.get('/', (req, res) => {
    res.send('Hello World');
});

// Protected route
app.get('/protected', [keycloak.protect()], (req: any, res: any) => {
    const token = req.kauth.grant.access_token.content;
    console.log(token.name); // Use the token as required
    res.send('Secret stuff');
});

// Logout route
app.get('/logout', keycloak.protect(), (req: any, res) => {
    req.kauth.logout();
    res.redirect('/');
});

// Start the server
const port = 3000;
app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});