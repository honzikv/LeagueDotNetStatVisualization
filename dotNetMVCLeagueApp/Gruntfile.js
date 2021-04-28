const sass = require('node-sass');

module.exports = function (grunt) {
    
    'use strict';
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),

        sass: {
            options: {
                outputStyle: 'compressed',
                sourceMap: true,
                implementation: sass
            },
            dist: {
                files: [
                    {
                        expand: true,
                        cwd: 'wwwroot/scss',
                        src: ["**/*.sass", "**/*.scss"],
                        dest: 'wwwroot/css',
                        ext: '.css'
                    }
                ]
            }
        }
    });
    
    grunt.loadNpmTasks('grunt-sass');
    grunt.registerTask('default', ['sass']);
};