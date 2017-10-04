module.exports = function(grunt) {
	
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-contrib-jshint');
	grunt.loadNpmTasks('grunt-umd');
	
	grunt.initConfig({
		
		pkg: grunt.file.readJSON('package.json'),
		
		umd: {
			markerAnimate: {
				options: {
					src: 'jquery.easing.1.3.js',
					dest: 'dist/jquery.easing.1.3.umd.js',
					deps: {
						'default': ['jQuery'],
						amd: ['jquery'],
						cjs: ['jquery']
					},
					objectToExport: "jQuery"
				}
			}
		},

		uglify: {
			options: {
				sourceMap: true,
				banner: '/* <%= grunt.task.current.target %> v<%= pkg.version %> <%= grunt.template.today("dd-mm-yyyy") %> Node wrapper for jQuery Easing plugin (C) 2015 Terikon Software */\n'
			},
			markerAnimate: {
				src: 'dist/jquery.easing.1.3.umd.js',
				dest: 'dist/jquery.easing.1.3.umd.min.js'
			}
		},

		jshint: {
			all: ['Gruntfile.js', 'jquery.easing.1.3.js']
		}
		
	});
	
	grunt.registerTask('default', ['umd', 'uglify']);
};