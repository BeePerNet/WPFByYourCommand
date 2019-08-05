﻿using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

//[assembly: XmlnsDefinition("http://wpfbyyourcommand.codeplex.com", "WPFByYourCommand")]
[assembly: XmlnsDefinition("http://wpfbyyourcommand.codeplex.com", "WPFByYourCommand.Behaviors")]
[assembly: XmlnsDefinition("http://wpfbyyourcommand.codeplex.com", "WPFByYourCommand.Bindings")]
[assembly: XmlnsDefinition("http://wpfbyyourcommand.codeplex.com", "WPFByYourCommand.Commands")]
[assembly: XmlnsDefinition("http://wpfbyyourcommand.codeplex.com", "WPFByYourCommand.Controls")]
[assembly: XmlnsDefinition("http://wpfbyyourcommand.codeplex.com", "WPFByYourCommand.Converters")]
[assembly: XmlnsPrefix("http://wpfbyyourcommand.codeplex.com", "cmd")]


[assembly: CLSCompliant(true)]
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WPFByYourCommand")]
[assembly: AssemblyDescription("WPF Tools")]
#if DEBUG
[assembly: AssemblyConfiguration("DEBUG")]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#endif
[assembly: AssemblyCompany("Sébastien Gariépy")]
[assembly: AssemblyProduct("WPFByYourCommand")]
[assembly: AssemblyCopyright("GNU General Public License v3.0")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


// L'affectation de la valeur false à ComVisible rend les types invisibles dans cet assembly 
// aux composants COM.  Si vous devez accéder à un type dans cet assembly à partir de 
// COM, affectez la valeur true à l'attribut ComVisible sur ce type.
[assembly: ComVisible(false)]

//Pour commencer à générer des applications localisables, définissez
//<UICulture>CultureUtiliséePourCoder</UICulture> dans votre fichier .csproj
//dans <PropertyGroup>.  Par exemple, si vous utilisez le français
//dans vos fichiers sources, définissez <UICulture> à fr-FR. Puis, supprimez les marques de commentaire de
//l'attribut NeutralResourceLanguage ci-dessous. Mettez à jour "fr-FR" dans
//la ligne ci-après pour qu'elle corresponde au paramètre UICulture du fichier projet.

[assembly: NeutralResourcesLanguage("en")]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //où se trouvent les dictionnaires de ressources spécifiques à un thème
                                     //(utilisé si une ressource est introuvable dans la page,
                                     // ou dictionnaires de ressources de l'application)
    ResourceDictionaryLocation.SourceAssembly //où se trouve le dictionnaire de ressources générique
                                              //(utilisé si une ressource est introuvable dans la page,
                                              // dans l'application ou dans l'un des dictionnaires de ressources spécifiques à un thème)
)]


// Les informations de version pour un assembly se composent des quatre valeurs suivantes :
//
//      Version principale
//      Version secondaire
//      Numéro de build
//      Révision
//
// Vous pouvez spécifier toutes les valeurs ou indiquer les numéros de build et de révision par défaut
// en utilisant '*', comme indiqué ci-dessous :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.6.0.0")]
[assembly: AssemblyFileVersion("1.6.0.0")]

