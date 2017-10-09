/// <binding BeforeBuild='less' AfterBuild='copy-css' />
var gulp = require("gulp"),
    fs = require("fs"),
    less = require("gulp-less");


var sourcemaps = require('gulp-sourcemaps');


var concat = require('gulp-concat');


var cleanCSS = require('gulp-clean-css');


gulp.task("less", function () {
    return gulp.src('node_modules/font-awesome/less/font-awesome.less')
        .pipe(less())
        //.pipe(concat('font-awesome.min.css'))
        .pipe(gulp.dest('wwwroot/css'));
});


//var cssSrc = [
//    "node_modules/bootstrap/dist/css/bootstrap.css",
//    "node_modules/sb_admin/css/sb-admin.css",
//    //"node_modules/morris.js/morris.css",
//    'wwwroot/css/font-awesome.css',
//    "node_modules/jsgrid/dist/jsgrid.min.css",
//    "node_modules/jsgrid/dist/jsgrid-theme.min.css",
//    "style/cm.css"
//    //"node_modules/datatables/media/css/jquery.dataTables.min.css"
//];




//gulp.task("copy-css", function () {
//    return gulp.src(cssSrc)
//        .pipe(sourcemaps.init())
//        //.pipe(less())
//        .pipe(concat('base.min.css'))
//        .pipe(sourcemaps.write('.'))
//        //.pipe(cleanCSS())
//        .pipe(gulp.dest('wwwroot/css'));
//});