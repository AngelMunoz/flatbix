/** @type {import("snowpack").SnowpackUserConfig } */
const config = {
  mount: {
    public: { url: '/', static: true },
    src: { url: '/dist' },
  },
  plugins: [
    [
      '@snowpack/plugin-typescript'
    ],
  ],
  routes: [
    /* Enable an SPA Fallback in development: */
    // {"match": "routes", "src": ".*", "dest": "/index.html"},
  ],
  optimize: {
    /* Example: Bundle your final build: */
    // "bundle": true,
    bundle: true,
    minify: true,
    preload: true,
    sourcemap: 'external',
    splitting: true,
    treeshake: true,
    target: "es2017",
    manifest: true
  },
  packageOptions: {
    /* ... */
    types: true,
    stats: true
  },
  devOptions: {
    /* ... */
  },
  buildOptions: {
    /* ... */
  },
};


export default config;