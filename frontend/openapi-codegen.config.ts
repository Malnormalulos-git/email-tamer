import {
    generateSchemaTypes,
    generateReactQueryComponents,
} from '@openapi-codegen/typescript';
import { defineConfig } from '@openapi-codegen/cli';

export default defineConfig({
    emailTamerApi: {
        from: {
            source: 'url',
            url: 'http://localhost:5166/swagger/v1/swagger.json',
        },
        outputDir: './src/api',
        to: async (context) => {
            const filenamePrefix = 'emailTamerApi';
            const { schemasFiles } = await generateSchemaTypes(context, {
                filenamePrefix,
                useEnums: true,
            });
            await generateReactQueryComponents(context, {
                filenamePrefix,
                schemasFiles,
            });
        },
    },
});
