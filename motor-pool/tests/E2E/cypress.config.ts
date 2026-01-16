import {defineConfig} from 'cypress';

export default defineConfig({
    projectId: 'Motor Pool',
    e2e: {
        baseUrl: 'http://localhost:5260',
    },
    viewportHeight: 1080,
    viewportWidth: 1920,
});
