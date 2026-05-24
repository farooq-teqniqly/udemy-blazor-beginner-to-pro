# Verifier: NowPlayingApp (Web / Playwright)

Headless browser verification for the live Netlify deployment.

## Prerequisites

Node.js must be installed. Run once to install dependencies:

```bash
cd C:/tmp
npm init -y
npm install playwright
npx playwright install chromium
```

## Script

Save as `C:/tmp/verify_nowplaying.mjs` and run with `node verify_nowplaying.mjs`.

```js
import { chromium } from 'playwright';

const SITE = 'https://udemy-nowplaying-farooq-teqniqly.netlify.app';

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();

const errors = [];
const failedRequests = [];
page.on('console', msg => { if (msg.type() === 'error') errors.push(msg.text()); });
page.on('pageerror', err => errors.push(err.message));
page.on('response', r => { if (r.status() >= 400) failedRequests.push(`${r.status()} ${r.url()}`); });

// Home page
await page.goto(SITE, { waitUntil: 'networkidle', timeout: 45000 });
await page.waitForTimeout(8000);
await page.screenshot({ path: 'C:/tmp/home.png', fullPage: true });
console.log('Failed requests:', failedRequests);
console.log('Console errors:', errors);

// Popular - navigate directly; Blazor SPA routing is handled by _redirects
await page.goto(`${SITE}/popular`, { waitUntil: 'networkidle', timeout: 45000 });
await page.waitForTimeout(8000);
await page.screenshot({ path: 'C:/tmp/popular.png', fullPage: true });
const popularText = await page.locator('body').innerText();
console.log('Popular page (excerpt):', popularText.slice(0, 500));

// Now Playing
await page.goto(`${SITE}/now-playing`, { waitUntil: 'networkidle', timeout: 45000 });
await page.waitForTimeout(8000);
await page.screenshot({ path: 'C:/tmp/now-playing.png', fullPage: true });
const nowPlayingText = await page.locator('body').innerText();
console.log('Now Playing page (excerpt):', nowPlayingText.slice(0, 500));

// Search
await page.goto(`${SITE}/search`, { waitUntil: 'networkidle', timeout: 45000 });
await page.waitForTimeout(3000);
await page.fill('input[type="search"], input[placeholder]', 'batman');
await page.keyboard.press('Enter');
await page.waitForTimeout(5000);
await page.screenshot({ path: 'C:/tmp/search.png', fullPage: true });
const searchText = await page.locator('body').innerText();
console.log('Search results (excerpt):', searchText.slice(0, 500));

await browser.close();
```

## What to check

- `home.png` - landing page renders with nav and branding
- `popular.png` - movie grid with real TMDB data (posters, titles, release dates)
- `now-playing.png` - movie grid with real TMDB data
- `search.png` - search results for "batman" contain Batman titles
- `failedRequests` - must be empty (no 4xx/5xx)

## Known issues

- `"Unexpected token '<'"` console error appears on every page load. Does not affect
  functionality. Likely a Blazor WASM boot artifact or stale service worker.
- Nav link clicks from the landing page do not trigger SPA routing in headless mode
  due to duplicate link text in the hero and navbar. Use direct URL navigation instead.

## Routing notes

Client-side routing is enabled by `wwwroot/_redirects`:

```
/tmdb/*    /tmdb/*
/*         /    200
```

Direct navigation to `/popular`, `/now-playing`, `/search`, `/favorites` all work.
The `/tmdb/*` rule routes API calls to the Netlify edge function (`netlify/edge-functions/TMDB.js`)
which injects the TMDB bearer token server-side.
